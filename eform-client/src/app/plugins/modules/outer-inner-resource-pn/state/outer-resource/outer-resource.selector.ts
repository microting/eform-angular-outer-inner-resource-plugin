import {
  OuterInnerResourceState
} from '../outer-inner-resource.state';
import {createSelector} from '@ngrx/store';

export const selectInnerResourcePn = (state: {outerInnerResourcePn: OuterInnerResourceState}) => state.outerInnerResourcePn;
export const selectOuterResources =
  createSelector(selectInnerResourcePn, (state: OuterInnerResourceState) => state.outerResourcesState);
export const selectOuterResourcesPagination =
  createSelector(selectOuterResources, (state) => state.pagination);
export const selectOuterResourcesPaginationSort =
  createSelector(selectOuterResources, (state) => state.pagination.sort);
export const selectOuterResourcesPaginationIsSortDsc =
  createSelector(selectOuterResources, (state) => state.pagination.isSortDsc ? 'asc' : 'desc');
