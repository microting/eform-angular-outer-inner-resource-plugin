import {
  Component,
  EventEmitter,
  Inject,
  OnDestroy,
  OnInit,
} from '@angular/core';
import {OuterInnerResourcePnOuterResourceService} from '../../../../services';
import {OuterResourcePnModel} from '../../../../models';
import {AutoUnsubscribe} from 'ngx-auto-unsubscribe';
import {Subscription} from 'rxjs';
import {MAT_DIALOG_DATA, MatDialogRef} from '@angular/material/dialog';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-area-delete',
  templateUrl: './outer-resource-delete.component.html',
  styleUrls: ['./outer-resource-delete.component.scss'],
})
export class OuterResourceDeleteComponent implements OnInit, OnDestroy {
  onAreaDeleted: EventEmitter<void> = new EventEmitter<void>();

  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService,
    public dialogRef: MatDialogRef<OuterResourceDeleteComponent>,
    @Inject(MAT_DIALOG_DATA) public selectedAreaModel: OuterResourcePnModel = new OuterResourcePnModel(),
  ) {
  }

  deleteAreaSub$: Subscription;

  ngOnInit() {
  }

  ngOnDestroy() {
  }

  hide(result = false) {
    this.dialogRef.close(result);
  }

  deleteArea() {
    this.deleteAreaSub$ = this.machineAreaPnAreasService
      .deleteArea(this.selectedAreaModel.id)
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaDeleted.emit();
          this.selectedAreaModel = new OuterResourcePnModel();
          this.hide(true);
        }
      });
  }
}
