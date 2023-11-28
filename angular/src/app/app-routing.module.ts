import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { AppComponent } from './app.component';
import { AppRouteGuard } from '@shared/auth/auth-route-guard';
import { UsersComponent } from './users/users.component';
import { TenantsComponent } from './tenants/tenants.component';
import { RolesComponent } from 'app/roles/roles.component';
import { ChangePasswordComponent } from './users/change-password/change-password.component';
import { AssetComponent } from './asset/asset.component';
import { GroupComponent } from './group/group.component';
import { StatusTableComponent } from './status/status-table/status-table.component';
import { PackageComponent } from './package/package.component';
import { DashboardOverviewComponent } from './dashboard/dashboard-overview/dashboard-overview.component';
import { DashboardComponent } from './dashboard/dashboard/dashboard.component';

@NgModule({
    imports: [
        RouterModule.forChild([
            {
                path: '',
                component: AppComponent,
                children: [
                    { path: 'overview', component: GroupComponent, canActivate: [AppRouteGuard] },
                    { path: 'overview/:id', component: GroupComponent, canActivate: [AppRouteGuard] },
                    { path: 'overview/assets/:id', component: AssetComponent, canActivate: [AppRouteGuard] },
                    { path: 'overview/packages/:id', component: PackageComponent, canActivate: [AppRouteGuard] },
                    { path: 'table', component: StatusTableComponent, canActivate: [AppRouteGuard] },
                    { path: 'dashboard', component: DashboardOverviewComponent, canActivate: [AppRouteGuard] },
                    { path: 'dashboard/:id', component: DashboardComponent, canActivate: [AppRouteGuard] },
                    { path: 'users', component: UsersComponent, data: { permission: 'Pages.Users' }, canActivate: [AppRouteGuard] },
                    { path: 'roles', component: RolesComponent, data: { permission: 'Pages.Roles' }, canActivate: [AppRouteGuard] },
                    { path: 'tenants', component: TenantsComponent, data: { permission: 'Pages.Tenants' }, canActivate: [AppRouteGuard] },
                    { path: 'update-password', component: ChangePasswordComponent, canActivate: [AppRouteGuard] },
                ]
            }
        ])
    ],
    exports: [RouterModule]
})
export class AppRoutingModule { }
