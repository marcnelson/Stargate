import { Component, OnDestroy, OnInit } from '@angular/core';

@Component({
  selector: 'app-demo',
  templateUrl: './demo.component.html',
  styleUrls: ['./demo.component.scss']
})
export class DemoComponent implements OnInit, OnDestroy {

  options: any[] = []; // Array to store dropdown options retrieved from API
  selectedOption: any; // Property to store the selected option

  constructor() { }
  
  ngOnInit(){
    this.fetchOptions();
  }

  onClick(): void{
    alert("tada");
  }

  fetchOptions(): void {
    // Call your API service to fetch options
    // this.apiService.getOptions().subscribe((data: any[]) => {
    //   this.options = data; // Assign API response to options array
    // });
  }

  onSelectionChange(): void{
    alert("Do Something!  A record was just selected.")

  }

  ngOnDestroy(): void {
    
  }
}
