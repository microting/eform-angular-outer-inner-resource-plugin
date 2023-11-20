import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {TranslateModule} from '@ngx-translate/core';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
} from './components';
import {InnerResourcesRouting} from './inner-resources.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {FormsModule} from '@angular/forms';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatButtonModule} from '@angular/material/button';
import {MtxGridModule} from '@ng-matero/extensions/grid';
import {MatDialogModule} from '@angular/material/dialog';
import {MatFormFieldModule} from '@angular/material/form-field';
import {MatInputModule} from '@angular/material/input';

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
    InnerResourcesRouting,
    EformSharedModule,
    FormsModule,
    MatTooltipModule,
    MatButtonModule,
    MtxGridModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  providers: [],
})
export class InnerResourcesModule {
}
