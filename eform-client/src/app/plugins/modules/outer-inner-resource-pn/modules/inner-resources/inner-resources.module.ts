import {CommonModule} from '@angular/common';
import {NgModule} from '@angular/core';
import {RouterModule} from '@angular/router';
import {TranslateModule} from '@ngx-translate/core';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
} from './components';
import {InnerResourcesRouting} from './inner-resources.routing';
import {EformSharedModule} from 'src/app/common/modules/eform-shared/eform-shared.module';
import {innerResourcesPersistProvider} from './components/store';
import {FormsModule} from '@angular/forms';
import {MatTooltipModule} from '@angular/material/tooltip';
import {MatButtonModule} from '@angular/material/button';
import {MtxGridModule} from '@ng-matero/extensions/grid';
import {MatCheckboxModule} from '@angular/material/checkbox';
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
    RouterModule,
    InnerResourcesRouting,
    EformSharedModule,
    FormsModule,
    MatTooltipModule,
    MatButtonModule,
    MtxGridModule,
    MatCheckboxModule,
    MatDialogModule,
    MatFormFieldModule,
    MatInputModule,
  ],
  providers: [innerResourcesPersistProvider],
})
export class InnerResourcesModule {
}
