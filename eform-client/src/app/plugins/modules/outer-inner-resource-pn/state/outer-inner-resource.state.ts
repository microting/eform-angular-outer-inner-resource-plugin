import {
  OuterResourcesState,
  InnerResourcesState
} from './';

export interface OuterInnerResourceState {
  innerResourcesState: InnerResourcesState;
  outerResourcesState: OuterResourcesState;
}
