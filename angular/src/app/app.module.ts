import {NgModule} from '@angular/core';
import {CommonModule} from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {HttpClientJsonpModule} from '@angular/common/http';
import {HttpClientModule} from '@angular/common/http';
import {ModalModule} from 'ngx-bootstrap/modal';
import {BsDropdownModule} from 'ngx-bootstrap/dropdown';
import {CollapseModule} from 'ngx-bootstrap/collapse';
import {TabsModule} from 'ngx-bootstrap/tabs';
import {NgxPaginationModule} from 'ngx-pagination';
import {AppRoutingModule} from './app-routing.module';
import {AppComponent} from './app.component';
import {ServiceProxyModule} from '@shared/service-proxies/service-proxy.module';
import {SharedModule} from '@shared/shared.module';
import {CarouselModule} from 'ngx-bootstrap/carousel';
import {PaginationModule} from 'ngx-bootstrap/pagination';
import {TooltipModule} from 'ngx-bootstrap/tooltip';
import {AlertModule} from 'ngx-bootstrap/alert';
import {ProgressbarModule} from 'ngx-bootstrap/progressbar';

// tenants
import {TenantsComponent} from '@app/tenants/tenants.component';
import {CreateTenantDialogComponent} from './tenants/create-tenant/create-tenant-dialog.component';
import {EditTenantDialogComponent} from './tenants/edit-tenant/edit-tenant-dialog.component';
// roles
import {RolesComponent} from '@app/roles/roles.component';
import {CreateRoleDialogComponent} from './roles/create-role/create-role-dialog.component';
import {EditRoleDialogComponent} from './roles/edit-role/edit-role-dialog.component';
// users
import {UsersComponent} from '@app/users/users.component';
import {CreateUserDialogComponent} from '@app/users/create-user/create-user-dialog.component';
import {EditUserDialogComponent} from '@app/users/edit-user/edit-user-dialog.component';
import {ChangePasswordComponent} from './users/change-password/change-password.component';
import {ResetPasswordDialogComponent} from './users/reset-password/reset-password.component';
// layout
import {HeaderComponent} from './layout/header.component';
import {HeaderLeftNavbarComponent} from './layout/header-left-navbar.component';
import {HeaderLanguageMenuComponent} from './layout/header-language-menu.component';
import {HeaderUserMenuComponent} from './layout/header-user-menu.component';
import {FooterComponent} from './layout/footer.component';
import {SidebarComponent} from './layout/sidebar.component';
import {SidebarLogoComponent} from './layout/sidebar-logo.component';
import {SidebarUserPanelComponent} from './layout/sidebar-user-panel.component';
import {SidebarMenuComponent} from './layout/sidebar-menu.component';
import {HeaderModeToggleComponent} from './layout/header-mode-toggle.component';
import {AssetComponent} from './asset/asset.component';
import {StatusKpiComponent} from './status/status-kpi/status-kpi.component';
import {GroupComponent} from './group/group.component';
import {AssetSummaryComponent} from './asset/asset-summary/asset-summary.component';
import {GroupSummaryComponent} from './group/group-summary/group-summary.component';
import {StatusMessageComponent} from './status/status-message/status-message.component';
import {PathComponent} from './common/path/path.component';
import {CarouselComponent} from './common/carousel/carousel.component';
import {NgApexchartsModule} from 'ng-apexcharts';
import {ChartWrapperComponent} from './charts/chart-wrapper/chart-wrapper.component';
import {LineChartComponent} from './charts/custom-charts/line-chart.component';
import {AreaChartComponent} from './charts/custom-charts/area-chart.component';
import {BarChartComponent} from './charts/custom-charts/bar-chart.component';
import {PieChartComponent} from './charts/custom-charts/pie-chart.component';
import {DonutChartComponent} from './charts/custom-charts/donut-chart.component';
import {RadialBarChartComponent} from './charts/custom-charts/radial-bar-chart.component';
import {ScatterChartComponent} from './charts/custom-charts/scatter-chart.component';
import {BaseChartComponent} from './charts/base-chart/base-chart.component';
import {HeatMapChartComponent} from './charts/custom-charts/heat-map-chart.component';
import {RadarChartComponent} from './charts/custom-charts/radar-chart.component';
import {PolarAreaChartComponent} from './charts/custom-charts/polar-area-chart.component';
import {RangeAreaChartComponent} from './charts/custom-charts/range-area-chart.component';
import {TreeMapChartComponent} from './charts/custom-charts/tree-map-chart.component';
import {StatusTableComponent} from './status/status-table/status-table.component';
import {StatusTimelineComponent} from './status/status-timeline/status-timeline.component';
import {StatusTimelineButtonComponent} from './status/status-timeline/status-timeline-button/status-timeline-button.component';
import {StatusPreviewComponent} from './status/status-preview/status-preview.component';
import {StatusModalComponent} from './status/status-modal/status-modal.component';
import {CreateAssetModalComponent} from './edit/create-asset-modal/create-asset-modal.component';
import {CreateGroupModalComponent} from './edit/create-group-modal/create-group-modal.component';
import {CreatePackageModalComponent} from './edit/create-package-modal/create-package-modal.component';
import {EditPackageModalComponent} from './edit/edit-package-modal/edit-package-modal.component';
import {AssetImageCarouselComponent} from './asset/asset-image-carousel/asset-image-carousel.component';
import {PackageBarChartComponent} from './package/package-bar-chart/package-bar-chart.component';
import {DashboardComponent} from './dashboard/custom-dashboard/dashboard/dashboard.component';
import {DashboardOverviewComponent} from './dashboard/dashboard-overview/dashboard-overview.component';
import {CreateDashboardModalComponent} from './edit/create-dashboard-modal/create-dashboard-modal.component';
import {DashboardTileComponent} from './dashboard/custom-dashboard/dashboard-tile/dashboard-tile.component';
import {CreateDashboardTileModalComponent} from './edit/create-dashboard-tile-modal/create-dashboard-tile-modal.component';
import {PackagePreviewComponent} from './package/package-preview/package-preview.component';
import {PackageModalBase} from './edit/package-modal-base/package-modal-base.component';
import {SettingsComponent} from './settings/settings.component';
import {StatisticsDashboardComponent} from './dashboard/statistics-dashboards/statistics-dashboard/statistics-dashboard.component';
import {PackageStatisticsModalComponent} from './package/package-statistics-modal/package-statistics-modal.component';
import {StatisticsDashboardTileComponent} from './dashboard/statistics-dashboards/statistics-dashboard-tile/statistics-dashboard-tile.component';
import {StatisticsDashboardTimelineComponent} from './dashboard/statistics-dashboards/statistics-dashboard-timeline/statistics-dashboard-timeline.component';
import {LoadingAnimationComponent} from './common/loading-animation/loading-animation.component';
import {HeaderAboutComponent} from './layout/header-about.component';
import {CriticalityIndicatorComponent} from './common/criticality-indicator/criticality-indicator.component';
import {RetentionSettingTileComponent} from './settings/retention-setting-tile/retention-setting-tile.component';
import {OpenAiKeySettingTileComponent} from './settings/open-ai-key-setting-tile/open-ai-key-setting-tile.component';
import {AssistantModalComponent} from './common/assistant-modal/assistant-modal.component';
import {AssistantButtonComponent} from './common/assistant-button/assistant-button.component';
import {StatusTableFilterComponent} from './status/status-table/status-table-filter/status-table-filter.component';
import {NoDataComponent} from './common/no-data/no-data.component';
import {RefreshBannerComponent} from './common/refresh-banner/refresh-banner.component';
import {StatusChartSectionComponent} from './status/status-chart-section/status-chart-section.component';
import {StatusMessageSectionComponent} from './status/status-message/status-message-section/status-message-section.component';
import {StatusKpiSectionComponent} from './status/status-kpi/status-kpi-section/status-kpi-section.component';

@NgModule({
  declarations: [
    AppComponent,
    // tenants
    TenantsComponent,
    CreateTenantDialogComponent,
    EditTenantDialogComponent,
    // roles
    RolesComponent,
    CreateRoleDialogComponent,
    EditRoleDialogComponent,
    // users
    UsersComponent,
    CreateUserDialogComponent,
    EditUserDialogComponent,
    ChangePasswordComponent,
    ResetPasswordDialogComponent,
    // layout
    HeaderComponent,
    HeaderLeftNavbarComponent,
    HeaderLanguageMenuComponent,
    HeaderUserMenuComponent,
    FooterComponent,
    SidebarComponent,
    SidebarLogoComponent,
    SidebarUserPanelComponent,
    SidebarMenuComponent,
    HeaderModeToggleComponent,
    AssetComponent,
    StatusKpiComponent,
    GroupComponent,
    AssetSummaryComponent,
    GroupSummaryComponent,
    StatusMessageComponent,
    PathComponent,
    CarouselComponent,
    ChartWrapperComponent,
    LineChartComponent,
    AreaChartComponent,
    BarChartComponent,
    PieChartComponent,
    DonutChartComponent,
    RadialBarChartComponent,
    ScatterChartComponent,
    BaseChartComponent,
    HeatMapChartComponent,
    RadarChartComponent,
    PolarAreaChartComponent,
    RangeAreaChartComponent,
    TreeMapChartComponent,
    StatusTableComponent,
    StatusTimelineComponent,
    StatusTimelineButtonComponent,
    StatusPreviewComponent,
    StatusModalComponent,
    CreateAssetModalComponent,
    CreateGroupModalComponent,
    CreatePackageModalComponent,
    EditPackageModalComponent,
    AssetImageCarouselComponent,
    PackageBarChartComponent,
    DashboardComponent,
    DashboardOverviewComponent,
    CreateDashboardModalComponent,
    DashboardTileComponent,
    CreateDashboardTileModalComponent,
    PackagePreviewComponent,
    PackageModalBase,
    SettingsComponent,
    StatisticsDashboardComponent,
    PackageStatisticsModalComponent,
    StatisticsDashboardTileComponent,
    StatisticsDashboardTimelineComponent,
    LoadingAnimationComponent,
    HeaderAboutComponent,
    CriticalityIndicatorComponent,
    RetentionSettingTileComponent,
    OpenAiKeySettingTileComponent,
    AssistantModalComponent,
    AssistantButtonComponent,
    StatusTableFilterComponent,
    NoDataComponent,
    RefreshBannerComponent,
    StatusChartSectionComponent,
    StatusMessageSectionComponent,
    StatusKpiSectionComponent,
  ],
  imports: [
    CommonModule,
    CollapseModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpClientJsonpModule,
    ModalModule.forChild(),
    BsDropdownModule,
    CollapseModule,
    TabsModule,
    AppRoutingModule,
    ServiceProxyModule,
    SharedModule,
    NgxPaginationModule,
    CarouselModule.forRoot(),
    NgApexchartsModule,
    PaginationModule,
    TooltipModule,
    AlertModule.forRoot(),
    ProgressbarModule.forRoot(),
  ],
  providers: [],
})
export class AppModule {}
