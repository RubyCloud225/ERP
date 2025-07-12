import { Component, OnInit } from '@angular/core';
import { AuthStateService } from '../../../core/services/auth-state.service';
import { HttpClient } from '@angular/common/http';
import { ChartConfiguration, ChartOptions } from 'chart.js';

@Component({
  selector: 'app-features-section',
  templateUrl: './features-section.component.html',
  styleUrls: ['./features-section.component.scss']
})
export class FeaturesSectionComponent implements OnInit {
  cards = [
    { id: 1, title: 'Card 1', size: 'small', content: 'This is a small card.' },
    { id: 2, title: 'Card 2', size: 'large', content: 'This is a large card with more content.' },
    { id: 3, title: 'Card 3', size: 'small', content: 'Another small card.' },
    { id: 4, title: 'Sales Growth Rate', size: 'large', content: 'chart' },
    { id: 5, title: 'Card 5', size: 'small', content: 'Small card again.' }
  ];

  showDashboard: boolean = false;

  salesGrowthData: ChartConfiguration<'line'>['data'] = {
    labels: [],
    datasets: [
      {
        data: [],
        label: 'Sales Growth Rate',
        fill: true,
        borderColor: '#42A5F5',
        backgroundColor: 'rgba(66, 165, 245, 0.2)',
        tension: 0.4
      }
    ]
  };

  salesGrowthOptions: ChartOptions<'line'> = {
    responsive: true,
    plugins: {
      legend: {
        display: true
      }
    }
  };

  constructor(private authStateService: AuthStateService, private http: HttpClient) {}

  ngOnInit(): void {
    this.authStateService.showDashboard$.subscribe(show => {
      this.showDashboard = show;
    });
    this.loadSalesGrowthData();
  }

  loadSalesGrowthData(): void {
    this.http.get<{ labels: string[], data: number[] }>('/api/sales-growth-rate').subscribe({
      next: (response) => {
        this.salesGrowthData.labels = response.labels;
        this.salesGrowthData.datasets[0].data = response.data;
      },
      error: (error) => {
        console.error('Failed to load sales growth data', error);
      }
    });
  }
}
