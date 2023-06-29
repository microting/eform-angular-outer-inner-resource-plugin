import {
  Component,
  EventEmitter, Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {InnerResourcePnModel,} from '../../../../models';
import { OuterInnerResourcePnInnerResourceService } from '../../../../services';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-machine-delete',
  templateUrl: './inner-resource-delete.component.html',
  styleUrls: ['./inner-resource-delete.component.scss'],
})
export class InnerResourceDeleteComponent implements OnInit, OnDestroy {
  onMachineDeleted: EventEmitter<void> = new EventEmitter<void>();

  deleteMachineSub$: Subscription;

  constructor(
    private machineAreaPnMachinesService: OuterInnerResourcePnInnerResourceService,
    public dialogRef: MatDialogRef<InnerResourceDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public selectedMachineModel: InnerResourcePnModel = new InnerResourcePnModel(),
  ) {}

  ngOnInit() {}

  ngOnDestroy() {}

  deleteMachine() {
    this.deleteMachineSub$ = this.machineAreaPnMachinesService
      .deleteMachine(this.selectedMachineModel.id)
      .subscribe((data) => {
        if (data && data.success) {
          this.onMachineDeleted.emit();
          this.selectedMachineModel = new InnerResourcePnModel();
          this.hide(true);
        }
      });
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }
}
