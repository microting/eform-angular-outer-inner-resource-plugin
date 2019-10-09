import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminGuard, AuthGuard} from 'src/app/common/guards';
import {OuterInnerResourcePnLayoutComponent} from './layouts';
import {InnerResourcesPageComponent, OuterResourcesPageComponent, ReportGeneratorContainerComponent} from './components';
import {OuterInnerResourceSettingsComponent} from './components/outer-inner-resource-settings';

export const routes: Routes = [
  {
    path: '',
    component: OuterInnerResourcePnLayoutComponent,
    children: [
      {
        path: 'InnerResources',
        canActivate: [AuthGuard],
        component: InnerResourcesPageComponent
      },
      {
        path: 'OuterResources',
        canActivate: [AdminGuard],
        component: OuterResourcesPageComponent
      },
      {
        path: 'settings',
        canActivate: [AdminGuard],
        component: OuterInnerResourceSettingsComponent
      },
      {
        path: 'reports',
        canActivate: [AdminGuard],
        component: ReportGeneratorContainerComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OuterInnerResourcePnRouting {
}
