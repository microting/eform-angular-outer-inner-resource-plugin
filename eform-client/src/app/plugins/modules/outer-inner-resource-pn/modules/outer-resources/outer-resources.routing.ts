import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from 'src/app/common/guards';
import {OuterResourcesPageComponent} from './components';

export const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: OuterResourcesPageComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class OuterResourcesRouting {
}
