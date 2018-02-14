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
                        title: "Menu",

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

                        contentSource:
                            Observable
                                .CombineLatest(
                                    LoginService.UserAccess.Select(v => v == ContentType.Premium),
                                    IsTutorialCompleted)
                                .Select<IList<bool>, ViewModel>(values => {
                                    var access = values[0];
                                    var tutorial = values[1];

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

            /*
			
			        <SwitchViewModel>

				        <SwitchItem
					        If={Bind MyLoginService.UserHasAccess(ContentType.Premium)}>
				
					        <LargeLoginViewModel 
						        LoginService={Bind MyLoginService} />			
				        </SwitchItem>
				
				        <SwitchItem
					        If={Bind !IsTutorialCompleted}>
				
					        <TutorialViewModel
						        OnCompleted={Bind IsTutorialCompleted.Set(true)} />			
				        </SwitchItem>

				        <SwitchItem>
				
					        <ContentBViewModel
						        DataSource={Bind ContentBDataSource1} />				
				        </SwitchItem>			
			        </SwitchViewModel>
		        </MultiViewPageViewModel>	
	        </App.StartupPage>
	
	        <App.Extensions>
		
		        <3rdPartyAdSdkIntegration
			        UseTestAds={Bind App.IsTestBuild & TestAdsRequested}
			        SdkKey="alksdfjl23k3"/>
		
		        <AnalyticsExtension
			        Endpoint={Bind MyAnalyticsEndpoint}/>
	        </App.Extensions>	 
            */
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
    }

    public static class HelpersExtension
    {

        public static IEnumerable<T> ToEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}
