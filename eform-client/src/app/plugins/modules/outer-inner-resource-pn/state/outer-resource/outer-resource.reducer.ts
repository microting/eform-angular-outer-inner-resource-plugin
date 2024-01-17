import {CommonPaginationState} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {
  updateOuterResourcePagination, updateOuterResourceTotal
} from './outer-resource.actions';

export interface OuterResourcesState {
  pagination: CommonPaginationState;
  total: number;
}

export const initialOuterResourcesState: OuterResourcesState = {
  pagination: {
    pageSize: 10,
    sort: 'Id',
    isSortDsc: false,
    offset: 0,
    total: 0,
    pageIndex: 0,
  },
  total: 0,
};

const _reducer = createReducer(
  initialOuterResourcesState,
  on(updateOuterResourcePagination, (state, {payload}) => ({
    ...state,
    pagination: {
      ...state.pagination,
      ...payload,
    },
  })),
  on(updateOuterResourceTotal, (state, {payload}) => ({
      ...state,
      total: payload,
      pagination: {
        ...state.pagination,
        total: payload,
      }
    }),
  ),
);


export function outerResourceReducer(state: OuterResourcesState | undefined, action: any) {
  return _reducer(state, action);
}
