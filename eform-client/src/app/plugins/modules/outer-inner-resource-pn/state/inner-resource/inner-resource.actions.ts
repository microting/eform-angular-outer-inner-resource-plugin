import {createAction} from '@ngrx/store';
import {CommonPaginationState} from 'src/app/common/models';

export const updateInnerResourcePagination = createAction(
  '[InnerResource] Update inner resource pagination',
  (payload: CommonPaginationState) => ({ payload })
);

export const updateInnerResourceTotal = createAction(
  '[InnerResource] Update inner resource total',
  (payload: number) => ({ payload })
);
