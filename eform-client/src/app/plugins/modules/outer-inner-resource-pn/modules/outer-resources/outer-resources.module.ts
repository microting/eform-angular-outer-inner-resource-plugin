import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {
  OwlDateTimeModule,
  OwlMomentDateTimeModule,
} from '@danielmoncada/angular-datetime-picker';
import {TranslateModule} from '@ngx-translate/core';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourcesPageComponent,
} from './components';
import {OuterResourcesRouting} from './outer-resources.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {EformSharedTagsModule} from 'src/app/common/modules/eform-shared-tags/eform-shared-tags.module';
import {outerResourcesPersistProvider} from './components/store';
import {MDBBootstrapModule} from 'angular-bootstrap-md';

@NgModule({
  declarations: [
    OuterResourceCreateComponent,
    OuterResourceDeleteComponent,
    OuterResourceEditComponent,
    OuterResourcesPageComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    RouterModule,
    OwlDateTimeModule,
    OuterResourcesRouting,
    OwlMomentDateTimeModule,
    EformSharedModule,
    FontAwesomeModule,
    FormsModule,
    ReactiveFormsModule,
    EformSharedTagsModule,
    MDBBootstrapModule,
  ],
  providers: [outerResourcesPersistProvider],
})
export class OuterResourcesModule {
}
