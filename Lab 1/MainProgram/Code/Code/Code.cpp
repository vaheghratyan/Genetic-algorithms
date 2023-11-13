#include <stdio.h>
#include <cstdio>
#include <stdlib.h>
#include <iostream>
#include <string>
#include <conio.h>
#include <fstream>
#include <io.h>
#include <time.h>
using namespace std;

const int Max = 100;
const int K = 50;
const int Mnogo = 30;

struct  TGorod {
	char Name[15];
	int  X, Y;
};


typedef int TOsob[Max];
TGorod G[Max];
TOsob pop[K];
double dl[K];
int N;


void Inic() {
	int sl;
	TOsob mas;
	srand(time(NULL));
	for (int i = 0; i < K; i++) {
		for (int j = 0; j < N; j++) mas[j] = j;
		for (int j = 0; j < N; j++) {
			sl = rand() % (N - j);
			pop[i][j] = mas[sl];
			for (int jj = sl + 1; jj < N; jj++) {
				mas[jj - 1] = mas[jj];
			}

		}

	}

}

void Selec()
{
	int nm;
	double s, min;
	int x1, x2, yl, y2;
	TOsob p;
	for (int i = 0; i < K; i++) {
		s = 0;
		for (int j = 0; j < N - 1; j++) {
			x1 = G[pop[i][j]].X;
			x2 = G[pop[i][j + 1]].X;
			yl = G[pop[i][j]].Y;
			y2 = G[pop[i][j + 1]].Y;
			s += sqrt(pow((x1 - x2), 2) + pow((yl - y2), 2));
		}
		dl[i] = s;
	}
	for (int i = 0; i < K; i++) {
		min = dl[i];
		nm = i;
		for (int j = i + 1; j < K; j++) {
			if (dl[j] < min) {
				min = dl[j];
				nm = j;
			}
		}
		if (nm != i) {
			dl[nm] = dl[i];
			dl[i] = min;

			for (int y = 0; y < Max; y++) {
				p[y] = pop[nm][y];
				pop[nm][y] = pop[i][y];
				pop[i][y] = p[y];
			}
		}
	}

	for (int i = (K / 2); i < K; i++) {
		for (int j = 0; j < N; j++) {
			pop[i][j] = 0;
		}
	}

}

void Papa(int p, int r) {
	int sl, pos;
	TOsob mas;
	for (int j = 0; j < N; j++) {
		mas[j] = j;
	}
	for (int j = 0; j < N / 2; j++) {
		sl = rand() % (N - j);
		pos = mas[sl];
		pop[r][pos] = pop[p][pos];
		for (int i = sl + 1; i < N; i++) {
			mas[i - 1] = mas[i];
		}
	}
}

void Mama(int m, int r) {
	bool est;
	for (int i = 0; i < N; i++) {
		est = false;
		for (int j = 0; j < N; j++) {
			if (pop[r][j] == pop[m][i]) {
				est = true;
			}
		}
		if (!est) {
			int j = 0;
			do {
				j = j + 1;
			} while (pop[r][j] != 0);
			pop[r][j] = pop[m][i];
		}
	}

}



void Mut(int r) {
	int c, p1, p2;
	p1 = rand() % N;
	do {
		p2 = rand() % N;
	} while (p1 == p2);
	c = pop[r][p1];
	pop[r][p1] = pop[r][p2];
	pop[r][p2] = c;
}



void Scresh() {
	int r, m, p;
	for (r = (K / 2); r < K; r++) {
		p = rand() % (K / 2);
		do {
			m = rand() % (K / 2);
		} while (m == p);
		Papa(p, r); Mama(m, r); Mut(r);
	}
}



void main() {
	ifstream fin("goroda.txt");
	N = 0;
	while ((N < 12)) {
		fin >> G[N].Name;
		fin >> G[N].X;
		fin >> G[N].Y;
		N++;
	}
	Inic();
	for (int i = 0; i < Mnogo; i++) {
		Selec();
		Scresh();
	}
	Selec();
	for (int i = 0; i < N - 1; i++) {
		printf("%d-", pop[0][i]);
	}
	printf("%d\n", pop[0][N - 1]);
	for (int i = 0; i < N; i++) {
		printf("%s\n", G[pop[0][i]].Name);
	}
	printf("Расстояние: %f\n", dl[0]);
	system("pause");
}
