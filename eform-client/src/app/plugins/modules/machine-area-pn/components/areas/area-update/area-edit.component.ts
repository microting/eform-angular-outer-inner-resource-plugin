import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-area-edit',
  templateUrl: './area-edit.component.html',
  styleUrls: ['./area-edit.component.scss']
})
export class AreaEditComponent implements OnInit {
  @ViewChild('frame') frame;
  @Output() onNewAreaDeleted: EventEmitter<void> = new EventEmitter<void>();
  spinnerStatus = false;
  selectedAreaModel: {
    id: 1,
    name: ''
  };
  constructor() { }

  ngOnInit() {
  }

  show() {
    this.frame.show();
  }

  updateArea() {

  }
}
