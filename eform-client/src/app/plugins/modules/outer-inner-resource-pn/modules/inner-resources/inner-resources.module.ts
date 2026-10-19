import {NgModule} from '@angular/core';
import {MatDialogModule} from '@angular/material/dialog';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
} from './components';
import {InnerResourcesRouting} from './inner-resources.routing';

@NgModule({
  imports: [
    MatDialogModule,
    InnerResourceCreateComponent,
    InnerResourceDeleteComponent,
    InnerResourceEditComponent,
    InnerResourcesPageComponent,
    InnerResourcesRouting,
  ],
  providers: [],
})
export class InnerResourcesModule {
}
