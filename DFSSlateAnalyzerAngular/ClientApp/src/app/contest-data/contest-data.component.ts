import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IContest, IEntry, IPlayer, IEntryMember } from '../shared/interfaces';
import { DataService } from '../data.service';


@Component({
  selector: 'app-contest-data',
  templateUrl: './contest-data.component.html',
  styleUrls: ['./contest-data.component.css']
})
export class ContestDataComponent {
  public contests: IContest[] = [];
  public contests2: IContest[] = [];
  baseURL: string = 'Unknown';

  selectedEntry?: IEntry;
  onSelect(entry: IEntry): void {
    this.selectedEntry = entry;
  }

  getContests(): void {
    this.contests2 = this.dataService.getContests();
  }

  ngOnInit(): void {
    this.getContests();
  }


  constructor(private dataService: DataService, http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    http.get<IContest[]>('http://localhost:7271/contest'  ).subscribe(result => {
      this.contests = result
    }, error => console.error(error));
    this.baseURL = baseUrl;
  }
}

