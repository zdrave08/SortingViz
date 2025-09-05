# SortingViz – Sorting Algorithms Visualizer (WinForms, .NET 8)

**EN (English)**  
A lightweight Windows Forms app for teaching sorting algorithms through animation. It’s a C#/.NET 8 port of an older Java AWT project (by **Zdravko Lukić, 2015**), expanded with metrics (comparisons, swaps, time) and Big-O summaries, plus adjustable animation speed.

**SR (Srpski)**  
Lagani WinForms program za učenje sortiranja kroz animaciju. Port starijeg Java AWT projekta (autor **Zdravko Lukić, 2015**) u C#/.NET 8, proširen metrikama (poređenja, zamene, vreme) i Big-O pregledom, uz klizač za brzinu animacije.

---

## ✨ Features / Karakteristike

- **Live visualization / Vizuelizacija u realnom vremenu**
- **Metrics:** comparisons, swaps, steps, elapsed time  
  **Metrike:** poređenja, zamene, koraci, trajanje
- **Big-O panel:** Best / Average / Worst / Space  
  **Big-O panel:** Najbolji / Prosek / Najgori / Memorija
- **Speed control / Kontrola brzine:** delay slider (0–200 ms)
- **Pluggable algorithms / Lako dodavanje algoritama**

---

## ✅ Supported algorithms / Podržani algoritmi

- Bubble, Insertion, Selection  
- Quick, Shell, **Heap**, **Merge** (crveno/plavo bojenje podnizova), **Radix (LSD)**

---

## 📦 Requirements / Zahtevi

- **Windows**
- **.NET 8 SDK** (and Visual Studio 2022+ or `dotnet` CLI)

---

## 🚀 Run / Pokretanje

1) Clone or copy the project / Kloniraj ili kopiraj projekat  
2) Open in Visual Studio **or** from terminal:  
   ```bash
   dotnet build
   dotnet run
   ```
3) The app starts with the main UI form (SortingViz).
   Aplikacija se pokreće sa glavnom formom (SortingViz).

## Program.cs - example

```
using System.Windows.Forms;

ApplicationConfiguration.Initialize();
Application.Run(new SortingViz(new SortAlgorithm[]
{
    new BubbleSort(),
    new InsertionSort(),
    new SelectionSort(),
    new QuickSort(),
    new ShellSort(),
    new HeapSort(),
    new MergeSort(),
    new RadixSort()
}));
```

## 🧠 How it works / Kako radi

- BufferedCanvas: custom double-buffer control (off-screen Bitmap), safe drawing API (WithGraphics(Action<Graphics>)) i Present() za repaint.

- VisualSortArray: čuva vrednosti, crta stubce, prati metrike, izlaže Compare, Swap, Highlight, SmallDelay, Values (za ne-komparativne algoritme kao Radix/Merge).

- SortAlgorithm: apstraktna baza; algoritam implementira Sort(VisualSortArray) i Big-O stringove.

- SortThread: izvršava sortiranje u pozadinskoj niti i javlja završetak (UI osveži metrike).

- SortingViz: glavni UI (dropdown za algoritam, Size/Scale, Delay slider, metrics & Big-O).

- SortFrame: hostuje jedan canvas i uredno zaustavlja sort nit pri zatvaranju.

## ⏱️ Speed control / Kontrola brzine

- Delay slider (0–200 ms) u gornjoj traci.

- Interno: VisualSortArray.SetDelay(ms) → SmallDelay() se poziva (tipično posle Swap, opciono i u Compare).

Tip: Za veće nizove, 10–30 ms je jasno; za demonstracije 50–100 ms.

## ➕ Add a new algorithm / Dodavanje novog algoritma

1) Napravi klasu u Algorithms/ koja nasleđuje SortAlgorithm.

2) Implementiraj:

- public override string Name => "YourAlgo";

- Big-O stringove

- public override void Sort(VisualSortArray a) koristeći a.Compare(i,j), a.Swap(i,j), opciono a.Highlight(...), a.SmallDelay()

3) Dodaj u listu u Program.cs.

4) (Opc.) ubaci Big-O red u rečnik u MagicSort.

Primer
```
public sealed class MySort : SortAlgorithm
{
    public override string Name => "MySort";
    public override string ComplexityBest => "O(n log n)";
    public override string ComplexityAverage => "O(n log n)";
    public override string ComplexityWorst => "O(n²)";

    public override void Sort(VisualSortArray a)
    {
        // use a.Compare, a.Swap, a.Highlight, a.SmallDelay
    }
}
```

## 🔧 Troubleshooting / Rešavanje problema

 - *System.ArgumentException: “Parameter is not valid.”*
Najčešće crtanje preko zastarelog/odbačenog Graphics posle resize-a.
✔️ Koristi BufferedCanvas.WithGraphics(...) (uvek svež Graphics i lock); ne keširaj Graphics.

- *InvalidOperationException: “Object is currently in use elsewhere.”*
UI nit slika _buffer dok pozadinska nit upisuje u isti bitmap.
✔️ Sav pristup _buffer je serijalizovan u WithGraphics; OnPaint koristi isti lock.

- *Cross-thread UI pozivi*
✔️ Present() je thread-safe (koristi BeginInvoke).

## 📚 Credits / Zahvale

Original Java concept & code: Zdravko Lukić (2015)

C#/.NET 8 WinForms port + features: collaborative refactor in this project

## 📄 License / Licenca

This project is licensed under the MIT License.
Ovaj projekat je licenciran pod MIT licencom.

See [LICENSE](https://github.com/zdrave08/SortingViz/blob/master/LICENSE.txt) for full text.
Pogledaj [LICENSE](https://github.com/zdrave08/SortingViz/blob/master/LICENSE.txt) za ceo tekst.

## 🗣️ Notes / Napomene

- MergeSort: levo podpolje crveno, desno plavo tokom spajanja.

- Radix (LSD): podrazumeva ne-negativne vrednosti; negativni se mogu podržati podelom i spajanjem sekvenci.

- Scale utiče na širinu stubca i veličinu platna (size * scale).
