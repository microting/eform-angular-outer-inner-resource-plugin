import {
  Component,
  EventEmitter,
  OnDestroy,
  OnInit,
  Output,
  ViewChild,
} from '@angular/core';
import { OuterInnerResourcePnOuterResourceService } from '../../../../services';
import { OuterResourcePnModel } from '../../../../models';
import { AutoUnsubscribe } from 'ngx-auto-unsubscribe';
import { Subscription } from 'rxjs';

@AutoUnsubscribe()
@Component({
  selector: 'app-machine-area-pn-area-delete',
  templateUrl: './outer-resource-delete.component.html',
  styleUrls: ['./outer-resource-delete.component.scss'],
})
export class OuterResourceDeleteComponent implements OnInit, OnDestroy {
  @ViewChild('frame', { static: false }) frame;
  @Output() onAreaDeleted: EventEmitter<void> = new EventEmitter<void>();
  selectedAreaModel: OuterResourcePnModel = new OuterResourcePnModel();
  constructor(
    private machineAreaPnAreasService: OuterInnerResourcePnOuterResourceService
  ) {}

  deleteAreaSub$: Subscription;

  ngOnInit() {}

  ngOnDestroy() {}

  show(areaModel: OuterResourcePnModel) {
    this.selectedAreaModel = areaModel;
    this.frame.show();
  }

  deleteArea() {
    this.deleteAreaSub$ = this.machineAreaPnAreasService
      .deleteArea(this.selectedAreaModel.id)
      .subscribe((data) => {
        if (data && data.success) {
          this.onAreaDeleted.emit();
          this.selectedAreaModel = new OuterResourcePnModel();
          this.frame.hide();
        }
      });
  }
}
