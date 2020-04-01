import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {TranslateModule} from '@ngx-translate/core';
import {MDBBootstrapModule} from 'port/angular-bootstrap-md';
import {NgSelectModule} from '@ng-select/ng-select';
import {MY_MOMENT_FORMATS} from 'src/app/common/helpers';

import {
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnInnerResourceService, OuterInnerResourcePnReportsService,
  OuterInnerResourcePnSettingsService
} from './services';
import {OuterInnerResourcePnLayoutComponent} from './layouts';
import {SharedPnModule} from '../shared/shared-pn.module';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourcesPageComponent,
  OuterResourceEditComponent,
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourcesPageComponent,
  InnerResourceEditComponent,
  ReportPreviewTableComponent,
  ReportGeneratorFormComponent,
  OuterInnerResourceSettingsComponent,
  ReportGeneratorContainerComponent,
} from './components';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {OuterInnerResourcePnRouting} from './outer-inner-resource-pn-routing.module';


@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    OuterInnerResourcePnRouting,
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
    OuterResourcesPageComponent,
    OuterResourceDeleteComponent,
    OuterResourceCreateComponent,
    OuterResourceEditComponent,
    InnerResourcesPageComponent,
    InnerResourceCreateComponent,
    InnerResourceEditComponent,
    InnerResourceDeleteComponent,
    OuterInnerResourcePnLayoutComponent,
    OuterInnerResourceSettingsComponent,
    ReportGeneratorContainerComponent,
    ReportGeneratorFormComponent,
    ReportPreviewTableComponent
  ],
  providers: [
    OuterInnerResourcePnOuterResourceService,
    OuterInnerResourcePnInnerResourceService,
    OuterInnerResourcePnSettingsService,
    OuterInnerResourcePnReportsService,
  {provide: OWL_DATE_TIME_FORMATS, useValue: MY_MOMENT_FORMATS},
  ]
})
export class OuterInnerResourcePnModule { }
