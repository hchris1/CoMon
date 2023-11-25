import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientJsonpModule } from '@angular/common/http';
import { HttpClientModule } from '@angular/common/http';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { CollapseModule } from 'ngx-bootstrap/collapse';
import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgxPaginationModule } from 'ngx-pagination';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { ServiceProxyModule } from '@shared/service-proxies/service-proxy.module';
import { SharedModule } from '@shared/shared.module';
import { CarouselModule } from 'ngx-bootstrap/carousel';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AlertModule } from 'ngx-bootstrap/alert';

// tenants
import { TenantsComponent } from '@app/tenants/tenants.component';
import { CreateTenantDialogComponent } from './tenants/create-tenant/create-tenant-dialog.component';
import { EditTenantDialogComponent } from './tenants/edit-tenant/edit-tenant-dialog.component';
// roles
import { RolesComponent } from '@app/roles/roles.component';
import { CreateRoleDialogComponent } from './roles/create-role/create-role-dialog.component';
import { EditRoleDialogComponent } from './roles/edit-role/edit-role-dialog.component';
// users
import { UsersComponent } from '@app/users/users.component';
import { CreateUserDialogComponent } from '@app/users/create-user/create-user-dialog.component';
import { EditUserDialogComponent } from '@app/users/edit-user/edit-user-dialog.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { ResetPasswordDialogComponent } from './users/reset-password/reset-password.component';
// layout
import { HeaderComponent } from './layout/header.component';
import { HeaderLeftNavbarComponent } from './layout/header-left-navbar.component';
import { HeaderLanguageMenuComponent } from './layout/header-language-menu.component';
import { HeaderUserMenuComponent } from './layout/header-user-menu.component';
import { FooterComponent } from './layout/footer.component';
import { SidebarComponent } from './layout/sidebar.component';
import { SidebarLogoComponent } from './layout/sidebar-logo.component';
import { SidebarUserPanelComponent } from './layout/sidebar-user-panel.component';
import { SidebarMenuComponent } from './layout/sidebar-menu.component';
import { HeaderModeToggleComponent } from './layout/header-mode-toggle.component';
import { AssetComponent } from './asset/asset.component';
import { StatusKpiComponent } from './status/status-kpi/status-kpi.component';
import { GroupComponent } from './group/group.component';
import { AssetSummaryComponent } from './asset/asset-summary/asset-summary.component';
import { GroupSummaryComponent } from './group/group-summary/group-summary.component';
import { StatusMessageComponent } from './status/status-message/status-message.component';
import { PathComponent } from './common/path/path.component';
import { CarouselComponent } from './common/carousel/carousel.component';
import { NgApexchartsModule } from "ng-apexcharts";
import { ChartWrapperComponent } from './charts/chart-wrapper/chart-wrapper.component';
import { LineChartComponent } from './charts/custom-charts/line-chart.component';
import { AreaChartComponent } from './charts/custom-charts/area-chart.component';
import { BarChartComponent } from './charts/custom-charts/bar-chart.component';
import { PieChartComponent } from './charts/custom-charts/pie-chart.component';
import { DonutChartComponent } from './charts/custom-charts/donut-chart.component';
import { RadialBarChartComponent } from './charts/custom-charts/radial-bar-chart.component';
import { ScatterChartComponent } from './charts/custom-charts/scatter-chart.component';
import { BaseChartComponent } from './charts/base-chart/base-chart.component';
import { HeatMapChartComponent } from './charts/custom-charts/heat-map-chart.component';
import { RadarChartComponent } from './charts/custom-charts/radar-chart.component';
import { PolarAreaChartComponent } from './charts/custom-charts/polar-area-chart.component';
import { RangeAreaChartComponent } from './charts/custom-charts/range-area-chart.component';
import { TreeMapChartComponent } from './charts/custom-charts/tree-map-chart.component';
import { StatusTableComponent } from './status/status-table/status-table.component';
import { StatusTimelineComponent } from './status/status-timeline/status-timeline.component';
import { StatusTimelineButtonComponent } from './status/status-timeline/status-timeline-button/status-timeline-button.component';
import { StatusPreviewComponent } from './status/status-preview/status-preview.component';
import { StatusModalComponent } from './status/status-modal/status-modal.component';
import { CreateAssetModalComponent } from './edit/create-asset-modal/create-asset-modal.component';
import { CreateGroupModalComponent } from './edit/create-group-modal/create-group-modal.component';
import { CreatePackageModalComponent } from './edit/create-package-modal/create-package-modal.component';
import { EditPackageModalComponent } from './edit/edit-package-modal/edit-package-modal.component';
import { AssetImageCarouselComponent } from './asset/asset-image-carousel/asset-image-carousel.component';
import { PackageTimelineChartComponent } from './package/package-timeline-chart/package-timeline-chart.component';
import { PackageComponent } from './package/package.component';
import { PackageBarChartComponent } from './package/package-bar-chart/package-bar-chart.component';
import { PackageChartWrapperComponent } from './package/package-chart-wrapper/package-chart-wrapper.component';

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
        PackageTimelineChartComponent,
        PackageComponent,
        PackageBarChartComponent,
        PackageChartWrapperComponent
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
        AlertModule.forRoot()
    ],
    providers: []
})
export class AppModule { }
