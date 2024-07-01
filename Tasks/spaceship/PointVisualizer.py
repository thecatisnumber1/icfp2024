import os
import sys
import matplotlib.pyplot as plt
from matplotlib.widgets import Button
import tkinter as tk
from tkinter import simpledialog
import re

class PointVisualizer:
    def __init__(self, files):
        self.problems = files
        self.current_problem_index = 0
        
        self.fig, self.ax = plt.subplots(figsize=(10, 8))
        plt.subplots_adjust(bottom=0.2)
        
        self.prev_button_ax = plt.axes([0.2, 0.05, 0.1, 0.075])
        self.next_button_ax = plt.axes([0.7, 0.05, 0.1, 0.075])
        self.input_button_ax = plt.axes([0.35, 0.05, 0.1, 0.075])
        self.ship_path_button_ax = plt.axes([0.55, 0.05, 0.1, 0.075])
        
        self.prev_button = Button(self.prev_button_ax, 'Previous')
        self.next_button = Button(self.next_button_ax, 'Next')
        self.input_button = Button(self.input_button_ax, 'Input Data')
        self.ship_path_button = Button(self.ship_path_button_ax, 'Ship Path')
        
        self.prev_button.on_clicked(self.prev_problem)
        self.next_button.on_clicked(self.next_problem)
        self.input_button.on_clicked(self.open_input_dialog)
        self.ship_path_button.on_clicked(self.open_ship_path_dialog)
        
        self.fig.canvas.mpl_connect('key_press_event', self.on_key_press)
        
        self.input_points = []
        self.ship_path_points = []
        self.plot_current_problem()

    def load_points(self, filename):
        with open(filename, 'r') as f:
            return [list(map(float, line.strip().split())) for line in f]

    def plot_current_problem(self):
        self.ax.clear()
        
        if not self.problems:
            self.ax.text(0.5, 0.5, "No problem files found", ha='center', va='center')
        else:
            filename = self.problems[self.current_problem_index]
            points = self.load_points(filename)
            
            x, y = zip(*points)
            self.ax.scatter(x, y, c='blue', label='File points')
        
        if self.input_points:
            x, y = zip(*self.input_points)
            self.ax.plot(x, y, 'r-', label='Input points')
            self.ax.scatter(x, y, c='red', s=20)
        
        if self.ship_path_points:
            x, y = zip(*self.ship_path_points)
            self.ax.plot(x, y, 'g-', label='Ship path')
            self.ax.scatter(x, y, c='green', s=20)
        
        self.ax.set_title(f"Problem: {self.problems[self.current_problem_index]}")
        self.ax.set_xlabel("X")
        self.ax.set_ylabel("Y")
        self.ax.legend()
        self.fig.canvas.draw()

    def next_problem(self, event):
        self.current_problem_index = (self.current_problem_index + 1) % len(self.problems)
        self.plot_current_problem()

    def prev_problem(self, event):
        self.current_problem_index = (self.current_problem_index - 1) % len(self.problems)
        self.plot_current_problem()

    def on_key_press(self, event):
        if event.key == 'right':
            self.next_problem(event)
        elif event.key == 'left':
            self.prev_problem(event)

    def open_input_dialog(self, event):
        root = tk.Tk()
        root.withdraw()
        text = simpledialog.askstring("Input", "Paste your position data here:", parent=root)
        if text:
            self.process_input(text)

    def open_ship_path_dialog(self, event):
        root = tk.Tk()
        root.withdraw()
        text = simpledialog.askstring("Ship Path", "Paste your ship path data here:", parent=root)
        if text:
            self.process_ship_path(text)

    def process_input(self, text):
        self.input_points = []
        for line in text.split('\n'):
            match = re.search(r'pos = \((-?\d+(?:\.\d+)?), (-?\d+(?:\.\d+)?)\)', line)
            if match:
                x, y = map(float, match.groups())
                self.input_points.append((x, y))
        self.plot_current_problem()

    def process_ship_path(self, text):
        self.ship_path_points = []
        for line in text.split('\n'):
            match = re.search(r'Pos = \((-?\d+), (-?\d+)\)', line)
            if match:
                x, y = map(int, match.groups())
                self.ship_path_points.append((x, y))
        self.plot_current_problem()

def get_problem_files():
    return [f for f in os.listdir('.') if 'decoded' in f and os.path.isfile(f)]

if __name__ == "__main__":
    if len(sys.argv) > 1:
        files = [sys.argv[1]]
        if not os.path.isfile(files[0]):
            print(f"Error: File '{files[0]}' not found.")
            sys.exit(1)
    else:
        files = get_problem_files()
        if not files:
            print("No 'decoded' files found in the current directory.")
            sys.exit(1)
    
    visualizer = PointVisualizer(files)
    plt.show()