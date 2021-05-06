import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import {
  InnerResourcePnModel,
  InnerResourcesPnModel,
  OuterResourcesPnModel,
  OuterResourcesPnRequestModel,
} from '../../../models';
import {
  OuterInnerResourcePnInnerResourceService,
  OuterInnerResourcePnOuterResourceService,
} from '../../../services';
import { OuterInnerResourcePnClaims } from '../../../enums';
import { Subscription } from 'rxjs';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { InnerResourcesStateService } from '../store';
import { TableHeaderElementModel } from 'src/app/common/models';
import { AuthStateService } from 'src/app/common/store';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machines-page',
  templateUrl: './inner-resources-page.component.html',
  styleUrls: ['./inner-resources-page.component.scss'],
})
export class InnerResourcesPageComponent implements OnInit, OnDestroy {
  @ViewChild('createMachineModal', { static: false }) createMachineModal;
  @ViewChild('editMachineModal', { static: false }) editMachineModal;
  @ViewChild('deleteMachineModal', { static: false }) deleteMachineModal;
  machinesModel: InnerResourcesPnModel = new InnerResourcesPnModel();
  mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel();
  name: string;

  getAllMachinesSub$: Subscription;
  getAllAreasSub$: Subscription;

  tableHeaders: TableHeaderElementModel[] = [
    { name: 'Id', elementId: 'idTableHeader', sortable: true },
    { name: 'Name', elementId: 'nameTableHeader', sortable: true },
    {
      name: 'ExternalId',
      elementId: 'externalIdTableHeader',
      sortable: true,
      visibleName: 'External ID',
    },
    { name: 'Actions', elementId: '', sortable: false },
  ];

  get outerInnerResourcePnClaims() {
    return OuterInnerResourcePnClaims;
  }

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    public innerResourcesStateService: InnerResourcesStateService,
    public authStateService: AuthStateService
  ) {}

  get currentRole(): string {
    return this.authStateService.currentRole;
  }

  ngOnInit() {
    this.getAllInitialData();
  }

  ngOnDestroy() {}

  getAllInitialData() {
    this.getAllMachines();
    this.getMappedAreas();
  }

  getAllMachines() {
    this.getAllMachinesSub$ = this.innerResourcesStateService
      .getAllMachines()
      .subscribe((data) => {
        if (data && data.success) {
          this.machinesModel = data.model;
        }
      });
  }

  getMappedAreas() {
    this.getAllAreasSub$ = this.machineAreaPnAreasService
      .getAllAreas(new OuterResourcesPnRequestModel())
      .subscribe((data) => {
        if (data && data.success) {
          this.mappingAreas = data.model;
        }
      });
  }

  showEditMachineModal(machine: InnerResourcePnModel) {
    this.editMachineModal.show(machine);
  }

  showDeleteMachineModal(machine: InnerResourcePnModel) {
    this.deleteMachineModal.show(machine);
  }

  showCreateMachineModal() {
    this.createMachineModal.show();
  }

  sortTable(sort: string) {
    this.innerResourcesStateService.onSortTable(sort);
    this.getAllMachines();
  }

  changePage(offset: number) {
    this.innerResourcesStateService.changePage(offset);
    this.getAllMachines();
  }

  onMachineDeleted() {
    this.innerResourcesStateService.onDelete();
    this.getAllMachines();
  }

  onPageSizeChanged(pageSize: number) {
    this.innerResourcesStateService.updatePageSize(pageSize);
    this.getAllMachines();
  }
}
