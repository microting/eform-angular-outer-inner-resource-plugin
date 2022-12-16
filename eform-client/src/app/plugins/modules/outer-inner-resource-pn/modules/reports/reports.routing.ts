import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AuthGuard} from 'src/app/common/guards';
import {ReportGeneratorContainerComponent} from './components';

export const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: ReportGeneratorContainerComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ReportsRouting {
}
