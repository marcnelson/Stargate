import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DemoService {

  constructor() { }

  getOptions(): string{
    return "Test Name";
  }
}
