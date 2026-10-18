import {NgModule} from '@angular/core';
import {
  OuterResourceCreateComponent,
  OuterResourceDeleteComponent,
  OuterResourceEditComponent,
  OuterResourcesPageComponent,
} from './components';
import {OuterResourcesRouting} from './outer-resources.routing';

@NgModule({
  imports: [
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
