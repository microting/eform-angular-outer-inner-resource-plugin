import {CommonPaginationState} from 'src/app/common/models';
import {createReducer, on} from '@ngrx/store';
import {
  updateInnerResourcePagination, updateInnerResourceTotal
} from './inner-resource.actions';

export interface InnerResourcesState {
  pagination: CommonPaginationState;
  total: number;
}

export const initialInnerResourcesState: InnerResourcesState = {
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
  initialInnerResourcesState,
  on(updateInnerResourcePagination, (state, {payload}) => ({
    ...state,
    pagination: {
      ...state.pagination,
      ...payload,
    },
  })),
  on(updateInnerResourceTotal, (state, {payload}) => ({
      ...state,
      total: payload,
      pagination: {
        ...state.pagination,
        total: payload,
      }
    }),
  ),
);

export function innerResourcesReducer(state: InnerResourcesState | undefined, action: any) {
  return _reducer(state, action);
}
