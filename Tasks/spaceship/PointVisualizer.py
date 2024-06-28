import os
import matplotlib.pyplot as plt
from matplotlib.widgets import Button

class PointVisualizer:
    def __init__(self):
        self.problems = self.get_problem_files()
        self.current_problem_index = 0
        
        self.fig, self.ax = plt.subplots(figsize=(10, 8))
        plt.subplots_adjust(bottom=0.2)
        
        self.prev_button_ax = plt.axes([0.3, 0.05, 0.1, 0.075])
        self.next_button_ax = plt.axes([0.6, 0.05, 0.1, 0.075])
        self.prev_button = Button(self.prev_button_ax, 'Previous')
        self.next_button = Button(self.next_button_ax, 'Next')
        
        self.prev_button.on_clicked(self.prev_problem)
        self.next_button.on_clicked(self.next_problem)
        
        self.fig.canvas.mpl_connect('key_press_event', self.on_key_press)
        
        self.plot_current_problem()
        
    def get_problem_files(self):
        return [f for f in os.listdir('.') if 'decoded' in f and os.path.isfile(f)]
    
    def load_points(self, filename):
        with open(filename, 'r') as f:
            return [list(map(float, line.strip().split())) for line in f]
    
    def plot_current_problem(self):
        if not self.problems:
            self.ax.text(0.5, 0.5, "No problem files found", ha='center', va='center')
            return
        
        filename = self.problems[self.current_problem_index]
        points = self.load_points(filename)
        
        self.ax.clear()
        x, y = zip(*points)
        self.ax.scatter(x, y)
        self.ax.set_title(f"Problem: {filename}")
        self.ax.set_xlabel("X")
        self.ax.set_ylabel("Y")
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

if __name__ == "__main__":
    visualizer = PointVisualizer()
    plt.show()