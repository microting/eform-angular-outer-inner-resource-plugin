import {NgModule} from '@angular/core';
import {
  InnerResourceCreateComponent,
  InnerResourceDeleteComponent,
  InnerResourceEditComponent,
  InnerResourcesPageComponent,
} from './components';
import {InnerResourcesRouting} from './inner-resources.routing';

@NgModule({
  imports: [
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
