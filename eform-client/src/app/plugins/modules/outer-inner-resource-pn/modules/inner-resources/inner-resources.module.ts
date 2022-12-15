import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {
  OwlDateTimeModule,
  OwlMomentDateTimeModule,
} from '@danielmoncada/angular-datetime-picker';
import {TranslateModule} from '@ngx-translate/core';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
} from './components';
import {InnerResourcesRouting} from './inner-resources.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {FontAwesomeModule} from '@fortawesome/angular-fontawesome';
import {EformSharedTagsModule} from 'src/app/common/modules/eform-shared-tags/eform-shared-tags.module';
import {innerResourcesPersistProvider} from './components/store';
import {MDBBootstrapModule} from 'angular-bootstrap-md';
import {FormsModule} from '@angular/forms';

@NgModule({
  declarations: [
    InnerResourceCreateComponent,
    InnerResourceDeleteComponent,
    InnerResourceEditComponent,
    InnerResourcesPageComponent,
  ],
  imports: [
    CommonModule,
    TranslateModule,
    RouterModule,
    OwlDateTimeModule,
    InnerResourcesRouting,
    OwlMomentDateTimeModule,
    EformSharedModule,
    FontAwesomeModule,
    EformSharedTagsModule,
    MDBBootstrapModule,
    FormsModule,
  ],
  providers: [innerResourcesPersistProvider],
})
export class InnerResourcesModule {
}
