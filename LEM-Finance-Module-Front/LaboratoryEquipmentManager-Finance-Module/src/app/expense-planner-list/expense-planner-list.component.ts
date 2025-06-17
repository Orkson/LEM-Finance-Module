import { Component, OnInit } from '@angular/core';
import { ApiServiceService } from '../api-service.service';
import { Router } from '@angular/router';


@Component({
  selector: 'app-expense-planner-list',
  templateUrl: './expense-planner-list.component.html',
  styleUrl: './expense-planner-list.component.css'
})
export class ExpensePlannerListComponent implements OnInit {
  expenses: any[] = [];
  selectedYear: number = new Date().getFullYear();
  summary: {[key: string]: { net: number; gross: number };} = {};

  totalNet: number = 0;
  totalGross: number = 0;

  constructor(private apiService: ApiServiceService, private router: Router) { }

  ngOnInit() {
    this.loadExpenses();
  }

  loadExpenses() {
    this.apiService.getExpensesByYear(this.selectedYear).subscribe({
        next: data => {
          this.expenses = data;
          console.log("Expenses:", this.expenses);
          this.calculateSummary();
        },
        error: err => {
          this.expenses = [];
          console.error(err);
        }
      });
      this.calculateSummary();
  }

  editExpense(expense: any) {
    console.log('Editing expense ID:', expense.id);
    this.router.navigate(['/expenses-planner-form', expense.id]);
  }

  deleteExpense(id: number) {
    if (confirm('Are you sure you want to delete this expense?')) {
      this.apiService.deleteExpense(id).subscribe({
        next: () => {
          console.log('Expense deleted:', id);
          this.loadExpenses();
        },
        error: (err) => console.error('Delete error:', err)
      });
    }
  }

  calculateSummary() {
    this.summary = {};
    this.totalNet = 0;
    this.totalGross = 0;

    this.expenses.forEach(expense => {
      const status = expense.status;

      if (!this.summary[status]) {
      this.summary[status] = { net: 0, gross: 0 };
      }

      this.summary[status].net += expense.netPricePLN;
      this.summary[status].gross += expense.grossPricePLN;

      this.totalNet += expense.netPricePLN;
      this.totalGross += expense.grossPricePLN;
  });
  }
}
