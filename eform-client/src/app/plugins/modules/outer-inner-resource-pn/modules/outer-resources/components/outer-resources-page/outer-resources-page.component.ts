import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  OuterResourcePnModel,
  OuterResourcesPnModel,
  InnerResourcesPnModel,
  InnerResourcesPnRequestModel,
} from '../../../../models';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
} from '../../../../services';
import { OuterResourcesStateService } from '../store';
import {PaginationModel, TableHeaderElementModel} from 'src/app/common/models';
import { Subscription } from 'rxjs';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import {Sort} from '@angular/material/sort';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService} from '@ngx-translate/core';

@AutoUnsubscribe()
@Component({
  selector: 'app-areas-page',
  templateUrl: './outer-resources-page.component.html',
  styleUrls: ['./outer-resources-page.component.scss'],
})
export class OuterResourcesPageComponent implements OnInit, OnDestroy {
  @ViewChild('createAreaModal', { static: false }) createAreaModal;
  @ViewChild('editAreaModal', { static: false }) editAreaModal;
  @ViewChild('deleteAreaModal', { static: false }) deleteAreaModal;
  areasModel: OuterResourcesPnModel = new OuterResourcesPnModel();
  mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel();

  getAllMachinesSub$: Subscription;
  getAllAreasSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id', sortProp: {id: 'Id'}, sortable: true},
    {header: this.translateService.stream('Name'), sortProp: {id: 'Name'}, field: 'name', sortable: true},
    {header: this.translateService.stream('External ID'), field: 'externalId', sortable: true, sortProp: {id: 'ExternalId'}},
    {
      header: this.translateService.stream('Actions'), field: 'actions',
      type: 'button',
      buttons: [
        {
          type: 'icon',
          color: 'accent',
          icon: 'edit',
          click: (record) => this.showEditAreaModal(record),
          tooltip: this.translateService.stream('Edit'),
        },
        {
          type: 'icon',
          color: 'warn',
          icon: 'delete',
          click: (record) => this.showDeleteAreaModal(record),
          tooltip: this.translateService.stream('Delete'),
        },
      ]
    },
  ]

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    public outerResourcesStateService: OuterResourcesStateService,
    private translateService: TranslateService,
  ) {}

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {}

  getAllInitialData() {
    this.getAllAreas();
    this.getMachinesForMapping();
  }

  getAllAreas() {
    this.getAllAreasSub$ = this.outerResourcesStateService
      .getAllAreas()
      .subscribe((data) => {
        if (data && data.success) {
          this.areasModel = data.model;
        }
      });
  }

  getMachinesForMapping() {
    this.getAllMachinesSub$ = this.machineAreaPnMachinesService
      .getAllMachines(new InnerResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingMachines = data.model;
        }
      });
  }

  showEditAreaModal(area: OuterResourcePnModel) {
    this.editAreaModal.show(area);
  }

  showDeleteAreaModal(area: OuterResourcePnModel) {
    this.deleteAreaModal.show(area);
  }

  showCreateAreaModal() {
    this.createAreaModal.show();
  }

  sortTable(sort: Sort) {
    this.outerResourcesStateService.onSortTable(sort.active);
    this.getAllAreas();
  }

  changePage(offset: number) {
    this.outerResourcesStateService.changePage(offset);
    this.getAllAreas();
  }

  onPageSizeChanged(pageSize: number) {
    this.outerResourcesStateService.updatePageSize(pageSize);
    this.getAllAreas();
  }

  onAreaDeleted() {
    this.outerResourcesStateService.onDelete();
    this.getAllAreas();
  }

  onPaginationChanged(paginationModel: PaginationModel) {
    this.outerResourcesStateService.updatePagination(paginationModel);
    this.getAllAreas();
  }
}
