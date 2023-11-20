import {
  OuterResourcesState
} from './outer-resource/outer-resource.reducer';
import {
  InnerResourcesState
} from './inner-resource/inner-resource.reducer';

export interface OuterInnerResourceState {
  innerResourcesState: InnerResourcesState;
  outerResourcesState: OuterResourcesState;
}
