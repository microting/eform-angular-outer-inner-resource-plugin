import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {MDBBootstrapModule} from 'port/angular-bootstrap-md';
import {NgSelectModule} from '@ng-select/ng-select';

import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService,
  MachineAreaPnSettingsService
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
import { MachineAreaSettingsComponent } from './components/machine-area-settings';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    MachineAreaPnRouting,
    TranslateModule,
    FormsModule,
    NgSelectModule
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
    MachineAreaPnLayoutComponent,
    MachineAreaSettingsComponent
  ],
  providers: [MachineAreaPnAreasService, MachineAreaPnMachinesService,   MachineAreaPnSettingsService
  ]
})
export class MachineAreaPnModule { }
