import {
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  InnerResourcesPnModel,
  OuterResourcePnCreateModel,
} from '../../../../models';
import {OuterInnerResourcePnOuterResourceService} from '../../../../services';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {Subscription} from 'rxjs';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';
import {TranslateService} from '@ngx-translate/core';

@AutoUnsubscribe()
@Component({
    selector: 'app-machine-area-pn-area-create',
    templateUrl: './outer-resource-create.component.html',
    styleUrls: ['./outer-resource-create.component.scss'],
    standalone: false
})
export class OuterResourceCreateComponent implements OnInit, OnDestroy {
  onAreaCreated: EventEmitter<void> = new EventEmitter<void>();
  newAreaModel: OuterResourcePnCreateModel = new OuterResourcePnCreateModel();

  createAreaSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id',},
    {header: this.translateService.stream('Name'), field: 'name',},
    {header: this.translateService.stream('Relationship'), field: 'relationship',},
  ];

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    private translateService: TranslateService,
    public dialogRef: MatDialogRef<OuterResourceCreateComponent>,
    @Inject(MAT_DIALOG_DATA) public mappingMachines: InnerResourcesPnModel = new InnerResourcesPnModel()
  ) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  createArea() {
    this.createAreaSub$ = this.machineAreaPnAreasService
      .createArea(this.newAreaModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaCreated.emit();
          this.newAreaModel = new OuterResourcePnCreateModel();
          this.hide(true);
        }
      });
  }

  addToArray(checked: boolean, machineId: number) {
    if (checked) {
      this.newAreaModel.relatedInnerResourcesIds.push(machineId);
    } else {
      this.newAreaModel.relatedInnerResourcesIds = this.newAreaModel.relatedInnerResourcesIds.filter(
        (x) => x !== machineId
      );
    }
  }

  isChecked(relatedMachineId: number) {
    return (
      this.newAreaModel.relatedInnerResourcesIds.indexOf(relatedMachineId) !==
      -1
    );
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }
}
