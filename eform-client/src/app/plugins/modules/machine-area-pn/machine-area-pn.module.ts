import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {OWL_DATE_TIME_FORMATS, OwlDateTimeModule, OwlNativeDateTimeModule} from 'ng-pick-datetime';
import {OwlMomentDateTimeModule} from 'ng-pick-datetime-moment';
import {MDBBootstrapModule} from 'port/angular-bootstrap-md';
import {NgSelectModule} from '@ng-select/ng-select';
import {MY_MOMENT_FORMATS} from 'src/app/common/helpers';

import {
  MachineAreaPnAreasService,
  MachineAreaPnMachinesService, MachineAreaPnReportsService,
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
  MachineEditComponent,
  ReportPreviewTableComponent,
  ReportGeneratorFormComponent,
  MachineAreaSettingsComponent,
  ReportGeneratorContainerComponent,
} from './components';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';


@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    MachineAreaPnRouting,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    OwlDateTimeModule,
    OwlNativeDateTimeModule,
    OwlMomentDateTimeModule,
    ReactiveFormsModule,
    FontAwesomeModule
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
    MachineAreaSettingsComponent,
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent
  ],
  providers: [
    MachineAreaPnAreasService,
    MachineAreaPnMachinesService,
    MachineAreaPnSettingsService,
    MachineAreaPnReportsService,
  {provide: OWL_DATE_TIME_FORMATS, useValue: MY_MOMENT_FORMATS},
  ]
})
export class MachineAreaPnModule { }
