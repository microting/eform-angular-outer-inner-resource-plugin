import {
  OuterInnerResourceState
} from '../outer-inner-resource.state';
import {createSelector} from '@ngrx/store';
import {AppState} from 'src/app/state';

const selectInnerResourcePn = (state: AppState & {outerInnerResourcePn: OuterInnerResourceState}) => state.outerInnerResourcePn;
export const selectInnerResources =
  createSelector(selectInnerResourcePn, (state: OuterInnerResourceState) => state.innerResourcesState);
export const selectInnerResourcesPagination =
  createSelector(selectInnerResources, (state) => state.pagination);
export const selectInnerResourcesPaginationSort =
  createSelector(selectInnerResources, (state) => state.pagination.sort);
export const selectInnerResourcesPaginationIsSortDsc =
  createSelector(selectInnerResources, (state) => state.pagination.isSortDsc ? 'asc' : 'desc');
