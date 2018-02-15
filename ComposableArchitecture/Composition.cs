using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace ComposableArchitecture
{
    class Composition
    {
        #region ids

        private const string MENU = "Menu";
        private const string DETAILS_PAGE = "DetailsPage";

        #endregion

        #region Backing fields

        private PageNavigationService _NavigationService;
        private PersistentValue<bool> _IsTutorialCompleted;
        private PersistentValue<bool> _TestAdsRequested;
        private ProdDevUri _LoginUri;
        private LoginService _LoginService;
        private NetworkDataSource<ContentA> _LatestContentAData;

        #endregion

        #region Persistent values

        private PersistentValue<bool> IsTutorialCompleted => _IsTutorialCompleted ?? (_IsTutorialCompleted =
            new PersistentValue<bool>("isTutorialCompleted")
        );

        private PersistentValue<bool> TestAdsRequested => _TestAdsRequested ?? (_TestAdsRequested =
            new PersistentValue<bool>("testAdsRequested")
        );

        #endregion

        #region URIs

        private static ProdDevUri LoginUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/login"),
                dev: new Uri("www.me-dev.com/login")
            );

        private static ProdDevUri AnalyticsUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/analytics"),
                dev: new Uri("www.me-dev.com/analytics")
            );

        private static ProdDevUri BaseContentUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/content/"),
                dev: new Uri("www.me-dev.com/content/")
            );

        #endregion

        private PageNavigationService NavigationService => _NavigationService ?? (_NavigationService =
            new PageNavigationService()
        );

        private LoginService LoginService => _LoginService ?? (_LoginService =
            new LoginService(
                endpoint: new LoginEndpoint(LoginUri)
            )
        );

        private MemoryCache ContentMemoryCache = new MemoryCache();

        private DiskCache ContentDiskCache = new DiskCache();

        private ContentAEndpoint LatestContentAEndpoint =
            new ContentAEndpoint(
                new RelativeUri(
                    BaseContentUri,
                    new Uri("latestcontenta")
                )
            );

        private NetworkDataSource<ContentA> LatestContentAData => _LatestContentAData ?? (_LatestContentAData =
            new NetworkDataSource<ContentA>(
                LatestContentAEndpoint,
                ContentMemoryCache,
                ContentDiskCache
            )
        );

        public PageViewModel Compose()
        {
            ExtensionsContainer
                .Instance
                .Add(

                    new MyAnalyticsExtension(AnalyticsUri) {
                        { MENU, "main-menu" },
                        { DETAILS_PAGE, "details-page-2" },
                    }
                
                );
         
            
            var latestContentAReloadCommand = 
                new UserCommandViewModel(
                    label: "Reload",
                    viewTemplate: "ReloadButton",
                    icon: Icon.Refresh,
                    actions: LatestContentAData.Reload
                );

            return
                new MultiViewPageViewModel(
                    new ViewViewModel(
                        title: "Localized menu title",
                        id: MENU,

                        content:
                        new ListViewModel(
                            viewTemplate: "MenuList",

                            new LoginViewModel(
                                LoginService
                            ),

                            new UserCommandViewModel(
                                label: "All the Content B!",
                                viewTemplate: "MenuButton",

                                actions: () => {
                                    var dataSource =
                                        new PagedNetworkDataSource<ContentB>(
                                            new ContentBEndpoint(
                                                new RelativeUri(
                                                    BaseContentUri,
                                                    new Uri("allthecontent")
                                                )
                                            )
                                        );

                                    dataSource.LoadNextPage();

                                    return NavigationService.Navigate(
                                        new SingleViewPageViewModel(
                                            id: DETAILS_PAGE,
                                            
                                            new ListViewModel(
                                                viewTemplate: "VerticalList",

                                                itemsSource:
                                                    dataSource
                                                        .Select<ContentB, ViewModel>(b => {
                                                            switch (b) {
                                                                case SuperContentB sb:
                                                                    return new SuperViewModel(sb, viewTemplate: "ContentBListItem");

                                                                default:
                                                                    return new ContentBListItemViewModel(b);
                                                            }
                                                        }),

                                                onRelativePositionChanged: 
                                                    rPos => {
                                                        if (rPos > 0.9 && !dataSource.IsLoading) {
                                                            dataSource.LoadNextPage();
                                                        }
                                                    }
                                            ),

                                            userCommands:
                                                new UserCommandViewModel(
                                                    "Reload",
                                                    "ReloadIcon",
                                                    Icon.Refresh,
                                                    dataSource.Reload
                                                )
                                        )
                                    );
                                }
                            ),

                            new UserCommandViewModel(
                                label: "Clear everything!",
                                viewTemplate: "MenuButtonWithLoadingIndicator",

                                actions: () =>
                                    Task.WhenAll(
                                        IsTutorialCompleted.Clear(),
                                        ContentMemoryCache.Clear(),
                                        ContentDiskCache.Clear()
                                    )
                            ),
#if DEBUG
                            new ListViewModel(
                                viewTemplate: "CompactVerticalList",

                                NewTestToggle("Ads", TestAdsRequested),
                                NewTestToggle("Content", BaseContentUri.UseDevelopment),
                                NewTestToggle("Login", LoginUri.UseDevelopment)
                            )
#endif
                        )
                    ),

                    new ViewViewModel(
                        title: "A Teaser",

                        new ListViewModel(
                            viewTemplate: "VerticalList",
                            
                            itemsSource:
                                LatestContentAData
                                    .Take(10)
                                    .Select(item => new SimpleListItemViewModel("TeaserListItem", item.Name, item.Text))
                                    .StartWith<ViewModel>(new LoginBannerViewModel(LoginService.IsUserLoggedIn))
                        ),

                        userCommands: latestContentAReloadCommand
                    ),


                    new ViewViewModel(
                        title: "All the A content",

                        userCommandsSource:
                            ForAccessToPremiumAndTutorialCompleted(
                                (access, tutorial) =>
                                    access && tutorial
                                        ? latestContentAReloadCommand.ToEnumerable()
                                        : Enumerable.Empty<UserCommandViewModel>()),

                        contentSource:
                            ForAccessToPremiumAndTutorialCompleted<ViewModel>(
                                (access, tutorial) => { 

                                    if (!access) {
                                        return 
                                            new LoginViewModel(
                                                LoginService,
                                                viewTemplate: "LargeLogin");
                                    }

                                    if (!tutorial) {
                                        return
                                            new TutorialViewModel(
                                                onCompleted: () => IsTutorialCompleted.Value = true
                                            );
                                    }

                                    return
                                        new ListViewModel(
                                            viewTemplate: "VerticalList",

                                            itemsSource:
                                                LatestContentAData
                                                    .Select<ContentA, ViewModel>(item => new ContentAViewModel(item))
                                        );
                                })
                            
                    )
                );

        }

        private IObservable<T> ForAccessToPremiumAndTutorialCompleted<T>(Func<bool, bool, T> select)
        {
            return
                Observable
                    .CombineLatest(
                        LoginService.UserAccess.Select(v => v == ContentType.Premium),
                        IsTutorialCompleted)
                    .Select(b => select(b[0], b[1]));
        }

        private ToggleViewModel NewTestToggle(string title, ISubject<bool> value)
        {
            return new ToggleViewModel(
                label: title,
                onLabel: "Test",
                offLabel: "Production",
                isOn: value
            );
        }


        private IObservable<T> C<T>(T instance)
        {
            return
                Observable.Return(instance);
        }

        private IObservable<T> C<T>(Func<T> factory)
        {
            return
                Observable.Create<T>(observer =>
                {
                    var value = factory();
                    observer.OnNext(value);
                    observer.OnCompleted();
                    return () => (value as IDisposable)?.Dispose();
                });
        }
    }

    public static class HelpersExtension
    {

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}
