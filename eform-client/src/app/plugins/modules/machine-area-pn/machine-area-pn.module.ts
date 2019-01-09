import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {MDBBootstrapModule} from 'port/angular-bootstrap-md';

import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService
} from './services';
import {MachineAreaPnLayoutComponent} from './layouts';
import {MachineAreaPnRouting} from './machine-area-pn-routing.module';
import {SharedPnModule} from '../shared/shared-pn.module';
import {
  AreaCreateComponent,
  AreaDeleteComponent,
  AreasPageComponent,
  AreaEditComponent,
  MachineCreateComponent,
  MachineDeleteComponent,
  MachinesPageComponent,
  MachineEditComponent
} from './components';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    MachineAreaPnRouting,
    TranslateModule,
    FormsModule
  ],
  declarations: [
    AreasPageComponent,
    AreaDeleteComponent,
    AreaCreateComponent,
    AreaEditComponent,
    MachinesPageComponent,
    MachineCreateComponent,
    MachineEditComponent,
    MachineDeleteComponent,
    MachineAreaPnLayoutComponent
  ],
  providers: [MachineAreaPnAreasService, MachineAreaPnMachinesService]
})
export class MachineAreaPnModule { }
