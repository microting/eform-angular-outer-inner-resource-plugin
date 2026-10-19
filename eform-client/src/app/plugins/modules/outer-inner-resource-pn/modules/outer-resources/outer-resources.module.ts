import {NgModule} from '@angular/core';
import {MatDialogModule} from '@angular/material/dialog';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourcesPageComponent,
} from './components';
import {OuterResourcesRouting} from './outer-resources.routing';

@NgModule({
  imports: [
    MatDialogModule,
    OuterResourceCreateComponent,
    OuterResourceDeleteComponent,
    OuterResourceEditComponent,
    OuterResourcesPageComponent,
    OuterResourcesRouting,
  ],
  providers: [],
})
export class OuterResourcesModule {
}
