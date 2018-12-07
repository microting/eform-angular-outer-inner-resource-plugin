import {Component, EventEmitter, OnInit, Output, ViewChild} from '@angular/core';

@Component({
  selector: 'app-machine-area-pn-area-delete',
  templateUrl: './area-delete.component.html',
  styleUrls: ['./area-delete.component.scss']
})
export class AreaDeleteComponent implements OnInit {
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

  deleteArea() {

  }
}
