import {Injectable} from '@angular/core';
import {
  CommonPaginationState,
  PaginationModel,
} from 'src/app/common/models';
import {updateTableSort} from 'src/app/common/helpers';
import {OuterInnerResourcePnInnerResourceService} from '../../../../services';
import {Store} from '@ngrx/store';
import {
  selectInnerResourcesPagination,
  updateInnerResourcePagination,
  updateInnerResourceTotal,
} from '../../../../state';
import {tap} from 'rxjs';

@Injectable({providedIn: 'root'})
export class InnerResourcesStateService {
  private selectInnerResourcesPagination$ = this.store.select(selectInnerResourcesPagination);
  currentPagination: CommonPaginationState;

  constructor(
    private store: Store,
    private service: OuterInnerResourcePnInnerResourceService,
  ) {
    this.selectInnerResourcesPagination$.subscribe(x => this.currentPagination = x);
  }

  getAllMachines() {
    return this.service
      .getAllMachines({
        ...this.currentPagination,
      }).pipe(
        tap((response) => {
          if (response && response.success && response.model) {
            this.store.dispatch(updateInnerResourceTotal(response.model.total));
          }
        })
      );
  }

  onDelete() {

    this.store.dispatch(updateInnerResourceTotal(this.currentPagination.total - 1));
  }

  onSortTable(sort: string) {
    const localPageSettings = updateTableSort(
      sort,
      this.currentPagination.sort,
      this.currentPagination.isSortDsc
    );
    this.store.dispatch(updateInnerResourcePagination({
      ...this.currentPagination,
      ...localPageSettings,
    }));
  }

  updatePagination(pagination: PaginationModel) {
    this.store.dispatch(updateInnerResourcePagination({
      ...this.currentPagination,
      pageSize: pagination.pageSize,
      offset: pagination.offset,
    }));
  }
}
