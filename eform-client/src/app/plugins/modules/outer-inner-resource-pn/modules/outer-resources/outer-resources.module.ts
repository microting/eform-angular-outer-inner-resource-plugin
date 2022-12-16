import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourcesPageComponent,
} from './components';
import {OuterResourcesRouting} from './outer-resources.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import {outerResourcesPersistProvider} from './components/store';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatButtonModule} from '@angular/material/button';
import {MtxGridModule} from '@ng-matero/extensions/grid';
import {MatDialogModule} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';
import {MatCheckboxModule} from '@angular/material/checkbox';

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
    OuterResourcesRouting,
    EformSharedModule,
    FormsModule,
    ReactiveFormsModule,
    MatTooltipModule,
    MatButtonModule,
    MtxGridModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
  ],
  providers: [outerResourcesPersistProvider],
})
export class OuterResourcesModule {
}
