import {FormsModule} from '@angular/forms';
import {MatFormField, MatLabel} from '@angular/material/form-field';
import {MatInput} from '@angular/material/input';
import {MtxGrid} from '@ng-matero/extensions/grid';
import {MatCheckbox} from '@angular/material/checkbox';
import {
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {
  InnerResourcePnCreateModel,
  OuterResourcesPnModel,
} from '../../../../models';
import {OuterInnerResourcePnInnerResourceService} from '../../../../services';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {Subscription} from 'rxjs';
import {MtxGridColumn} from '@ng-matero/extensions/grid';
import {TranslateService, TranslatePipe} from '@ngx-translate/core';
import {MAT_DIALOG_DATA, MatDialogRef, MatDialogTitle, MatDialogContent, MatDialogActions} from '@angular/material/dialog';

@AutoUnsubscribe()
@Component({
    selector: 'app-machine-area-pn-machine-create',
    templateUrl: './inner-resource-create.component.html',
    imports: [TranslatePipe, MatDialogTitle, MatDialogContent, MatDialogActions, FormsModule, MatFormField, MatLabel, MatInput, MtxGrid, MatCheckbox],
    styleUrls: ['./inner-resource-create.component.scss']
})
export class InnerResourceCreateComponent implements OnInit, OnDestroy {
  onMachineCreated: EventEmitter<void> = new EventEmitter<void>();
  checked = false;
  newMachineModel: InnerResourcePnCreateModel = new InnerResourcePnCreateModel();

  createMachineSub$: Subscription;

  tableHeaders: MtxGridColumn[] = [
    {header: this.translateService.stream('Id'), field: 'id',},
    {header: this.translateService.stream('Name'), field: 'name',},
    {header: this.translateService.stream('External ID'), field: 'externalId',},
    {header: this.translateService.stream('Relationship'), field: 'relationship',},
  ];

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    private translateService: TranslateService,
    public dialogRef: MatDialogRef<InnerResourceCreateComponent>,
    @Inject(MAT_DIALOG_DATA) public mappingAreas: OuterResourcesPnModel = new OuterResourcesPnModel()
  ) {
  }

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  createMachine() {
    this.createMachineSub$ = this.machineAreaPnMachinesService
      .createMachine(this.newMachineModel)
      .subscribe((data) => {
        if (data && data.success) {
          this.onMachineCreated.emit();
          this.newMachineModel = new InnerResourcePnCreateModel();
          this.hide(true);
        }
      });
  }

  addToArray(checked: boolean, areaId: number) {
    if (checked) {
      this.newMachineModel.relatedOuterResourcesIds.push(areaId);
    } else {
      this.newMachineModel.relatedOuterResourcesIds = this.newMachineModel.relatedOuterResourcesIds.filter(
        (x) => x !== areaId
      );
    }
  }

  isChecked(relatedAreaId: number) {
    return (
      this.newMachineModel.relatedOuterResourcesIds.indexOf(relatedAreaId) !==
      -1
    );
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }
}
