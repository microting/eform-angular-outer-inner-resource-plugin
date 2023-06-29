import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminGuard, AuthGuard, PermissionGuard} from 'src/app/common/guards';
import {OuterInnerResourcePnLayoutComponent} from './layouts';
import {OuterInnerResourceSettingsComponent} from './components';
import {OuterInnerResourcePnClaims} from './enums';

export const routes: Routes = [
  {
    path: '',
    component: OuterInnerResourcePnLayoutComponent,
    canActivate: [PermissionGuard],
    data: {
      requiredPermission:
      OuterInnerResourcePnClaims.accessOuterInnerResourcePlugin,
    },
    children: [
      {
        path: 'inner-resources',
        canActivate: [AuthGuard],
        loadChildren: () =>
          import('./modules/inner-resources/inner-resources.module').then(
            (m) => m.InnerResourcesModule
          ),
      },
      {
        path: 'outer-resources',
        canActivate: [AdminGuard],
        loadChildren: () =>
          import('./modules/outer-resources/outer-resources.module').then(
            (m) => m.OuterResourcesModule
          ),
      },
      {
        path: 'settings',
        canActivate: [AdminGuard],
        component: OuterInnerResourceSettingsComponent,
      },
      {
        path: 'reports',
        canActivate: [AdminGuard],
        loadChildren: () =>
          import('./modules/reports/reports.module').then(
            (m) => m.ReportsModule
          ),
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OuterInnerResourcePnRouting {
}
