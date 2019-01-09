import {NgModule} from '@angular/core';
import {RouterModule, Routes} from '@angular/router';
import {AdminGuard, AuthGuard} from 'src/app/common/guards';
import {MachineAreaPnLayoutComponent} from './layouts';
import {MachinesPageComponent, AreasPageComponent} from './components';

export const routes: Routes = [
  {
    path: '',
    component: MachineAreaPnLayoutComponent,
    children: [
      {
        path: 'machines',
        canActivate: [AuthGuard],
        component: MachinesPageComponent
      },
      {
        path: 'areas',
        canActivate: [AdminGuard],
        component: AreasPageComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class MachineAreaPnRouting {
}
