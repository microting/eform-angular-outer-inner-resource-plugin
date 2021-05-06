import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { NgSelectModule } from '@ng-select/ng-select';

import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
  OuterInnerResourcePnReportsService,
  OuterInnerResourcePnSettingsService,
} from './services';
import { OuterInnerResourcePnLayoutComponent } from './layouts';
import { SharedPnModule } from '../shared/shared-pn.module';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
  OuterInnerResourceSettingsComponent,
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourcesPageComponent,
  ReportGeneratorContainerComponent,
  ReportGeneratorFormComponent,
  ReportPreviewTableComponent,
} from './components';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { OuterInnerResourcePnRouting } from './outer-inner-resource-pn-routing.module';
import { OwlDateTimeModule } from '@danielmoncada/angular-datetime-picker';
import { EformSharedModule } from 'src/app/common/modules/eform-shared/eform-shared.module';
import { outerInnerResourcesStoreProviders } from './store-providers.config';
import { InnerResourcesStateService } from './components/inner-resources/store';
import { OuterResourcesStateService } from './components/outer-resources/store';

@NgModule({
  imports: [
    CommonModule,
    SharedPnModule,
    MDBBootstrapModule,
    OuterInnerResourcePnRouting,
    TranslateModule,
    FormsModule,
    NgSelectModule,
    ReactiveFormsModule,
    FontAwesomeModule,
    OwlDateTimeModule,
    EformSharedModule,
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
    ReportPreviewTableComponent,
  ],
  providers: [
    OuterInnerResourcePnOuterResourceService,
    OuterInnerResourcePnInnerResourceService,
    OuterInnerResourcePnSettingsService,
    OuterInnerResourcePnReportsService,
    InnerResourcesStateService,
    OuterResourcesStateService,
    ...outerInnerResourcesStoreProviders,
  ],
})
export class OuterInnerResourcePnModule {}
