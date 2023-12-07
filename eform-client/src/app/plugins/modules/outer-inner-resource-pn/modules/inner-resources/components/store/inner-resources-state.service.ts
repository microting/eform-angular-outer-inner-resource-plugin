import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  CommonPaginationState,
  OperationDataResult,
  PaginationModel,
} from 'src/app/common/models';
import { updateTableSort } from 'src/app/common/helpers';
import { map } from 'rxjs/operators';
import { OuterInnerResourcePnInnerResourceService } from '../../../../services';
import { InnerResourcesPnModel } from '../../../../models';
import {Store} from '@ngrx/store';
import {
  selectInnerResourcesPagination
} from '../../../../state/inner-resource/inner-resource.selector';

@Injectable({ providedIn: 'root' })
export class InnerResourcesStateService {
  private selectInnerResourcesPagination$ = this.store.select(selectInnerResourcesPagination);
  constructor(
    private store: Store,
    private service: OuterInnerResourcePnInnerResourceService,
  ) {}

  getAllMachines(): Observable<OperationDataResult<InnerResourcesPnModel>> {
    let pagination = new CommonPaginationState();
    this.selectInnerResourcesPagination$.subscribe(
      (state) => {
        pagination = state;
      }
    ).unsubscribe();
    return this.service
      .getAllMachines({
        ...pagination,
        // ...this.query.pageSetting.filters,
      }).pipe(
        map((response) => {
          if (response && response.success && response.model) {
            this.store.dispatch({
              type: '[InnerResource] Update inner resource pagination', payload: {
                pagination: {
                  ...pagination,
                  total: response.model.total,
                }
              }
            });
          }
          return response;
        })
      );
  }

  onDelete() {
    let currentPagination: CommonPaginationState;
    this.selectInnerResourcesPagination$.subscribe((pagination) => {
      if (pagination === undefined) {
        return;
      }
      currentPagination = pagination;
    }).unsubscribe();
    this.store.dispatch({
      type: '[InnerResource] Update inner resource pagination', payload: {
        pagination: {
          ...currentPagination,
          total: currentPagination.total - 1,
        }
      }
    });
  }

  onSortTable(sort: string) {
    let currentPagination: CommonPaginationState;
    this.selectInnerResourcesPagination$.subscribe((pagination) => {
      if (pagination === undefined) {
        return;
      }
      currentPagination = pagination;
    }).unsubscribe();
    const localPageSettings = updateTableSort(
      sort,
      currentPagination.sort,
      currentPagination.isSortDsc
    );
    this.store.dispatch({
      type: '[InnerResource] Update inner resource pagination', payload: {
        pagination: {
          ...currentPagination,
          sort: localPageSettings.sort,
          isSortDsc: localPageSettings.isSortDsc,
        }
      }
    });
  }

  updatePagination(pagination: PaginationModel) {
    let currentPagination: CommonPaginationState;
    this.selectInnerResourcesPagination$.subscribe((pagination) => {
      if (pagination === undefined) {
        return;
      }
      currentPagination = pagination;
    }).unsubscribe();
    this.store.dispatch({
      type: '[InnerResource] Update inner resource pagination', payload: {
        pagination: {
          ...currentPagination,
          pageSize: pagination.pageSize,
          offset: pagination.offset,
        }
      }
    });
  }
}
