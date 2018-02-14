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
        private PersistentBool _IsTutorialCompleted;
        private PersistentBool _TestAdsRequested;
        private ProdDevUri _LoginUri;
        private LoginService _LoginService;
        private NetworkDataSource<ContentA> _LatestContentAData;

        #endregion

        private PageNavigationService NavigationService => _NavigationService ?? (_NavigationService =
            new PageNavigationService()
        );

        private PersistentBool IsTutorialCompleted => _IsTutorialCompleted ?? (_IsTutorialCompleted =
            new PersistentBool("isTutorialCompleted")
        );

        private PersistentBool TestAdsRequested => _TestAdsRequested ?? (_TestAdsRequested =
            new PersistentBool("testAdsRequested")
        );

        private ProdDevUri LoginUri => _LoginUri ?? (_LoginUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/login"),
                dev: new Uri("www.me-dev.com/login")
            )
        );

        private LoginService LoginService => _LoginService ?? (_LoginService =
            new LoginService(
                endpoint: new LoginEndpoint(LoginUri)
            )
        );

        private ProdDevUri AnalyticsUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/analytics"),
                dev: new Uri("www.me-dev.com/analytics")
            );

        private MemoryCache ContentMemoryCache = new MemoryCache();

        private DiskCache ContentDiskCache = new DiskCache();

        private static ProdDevUri BaseContentUri =
            new ProdDevUri(
                prod: new Uri("www.me.com/content/"),
                dev: new Uri("www.me-dev.com/content/")
            );

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
            return
                new MultiViewPageViewModel(
                    new ViewViewModel(
                        title: "Menu",

                        new ListViewModel(
                            viewTemplate: "MenuList",

                            new SmallLoginViewModel(
                                LoginService
                            ),

                            new UserCommandViewModel(
                                label: "All the Content B!",
                                viewTemplate: "MenuButton",

                                () => {
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
                                                        .Items
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
                                                    dataSource.Reload
                                                )
                                        )
                                    );
                                }
                            ),

                            new UserCommandViewModel(
                                label: "Clear everything!",
                                viewTemplate: "MenuButtonWithLoadingIndicator",

                                () =>
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
                        title: "Content A",

                        new ListViewModel(
                            viewTemplate: "VerticalList",
                            
                            itemsSource: 
                                LatestContentAData
                                    .Items
                                    .Select(item => new SimpleListItemViewModel("BrandedListItem", item.Name, item.Text))
                                    .StartWith<ViewModel>(new LoginBannerViewModel(LoginService.IsUserLoggedIn))
                        ),

                        userCommands:
                            new UserCommandViewModel(
                                label: "Reload",
                                viewTemplate: "MenuButtonWithLoadingIndicator",
                                actions: LatestContentAData.Reload
                            )
                    )
                );

            /*
            
			        <ItemsViewModel
				        TemplateName="VerticalListTemplate"
				        ItemsSource={Bind ContentADataSource1.Take(10)}>
				
				        <ItemsViewModel.UserCommands>
					        <Run Action={Bind ContentADataSource1.Reload()}
				        </ItemsViewModel.UserCommands>
				
				        <ItemsViewModel.Header>
					        <LoginBannerViewModel 
						        IsVisible={Bind !MyLoginService.IsUserLoggedIn}
						        LoginService={Bind MyLoginService} />
				        </ItemsViewModel.Header>		
				
				        <ItemsViewModel.ItemTemplate>
					        <ContentAListItemViewModel
						        TemplateName="ContentATeaserTemplate" />
				        </ItemsViewModel.ItemTemplate>
				
			        </ItemsViewModel>
			
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
