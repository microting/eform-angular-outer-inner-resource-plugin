import {createAction} from '@ngrx/store';
import {CommonPaginationState} from 'src/app/common/models';

export const updateOuterResourcePagination = createAction(
  '[OuterResource] Update outer resource pagination',
  (payload: CommonPaginationState) => ({ payload })
);

export const updateOuterResourceTotal = createAction(
  '[OuterResource] Update outer resource total',
  (payload: number) => ({ payload })
);
