import matplotlib.pyplot as plt
import math

def main():
    input_file = "FPS_Report.txt"
    times = []
    fps_values = []

    with open(input_file, "r") as f:
        lines = f.readlines()

    # Skip the header line (assuming the first line is a header)
    for line in lines[1:]:
        line = line.strip()
        if not line:
            continue
        parts = line.split(';')
        if len(parts) == 2:
            t = float(parts[0])
            fps = float(parts[1])
            times.append(t)
            fps_values.append(fps)

    # Group FPS values by their integer second
    fps_by_second = {}
    for t, fps in zip(times, fps_values):
        second = int(math.floor(t))
        if second not in fps_by_second:
            fps_by_second[second] = []
        fps_by_second[second].append(fps)

    # Compute the average FPS for each second
    avg_times = sorted(fps_by_second.keys())
    avg_fps_values = [sum(fps_by_second[s]) / len(fps_by_second[s]) for s in avg_times]

    # Compute the overall average FPS
    if avg_fps_values:
        overall_avg_fps = sum(avg_fps_values) / len(avg_fps_values)
    else:
        overall_avg_fps = 0.0

    # Plot the averaged data
    plt.figure(figsize=(10, 2.5))
    plt.plot(avg_times, avg_fps_values, marker='o', linestyle='-', color='#0072BD', label='Avg FPS per Second')

    # Draw a horizontal red line for the overall average FPS
    plt.axhline(y=overall_avg_fps, color='#D95319', linestyle='--', linewidth=2, label=f'Avg FPS: {overall_avg_fps:.2f}')

    plt.title("FPS by Second")
    plt.xlabel("Time (s)")
    plt.ylabel("FPS")
    plt.grid(True)
    plt.legend(fontsize=14)

    # Set Y-axis limits
    plt.ylim(60, 300)  # Adjust as needed
    plt.xticks(range(0, 151, 50))
    plt.xlim(left=0)
    plt.xticks(fontsize=12)
    plt.yticks(fontsize=12)
    

    plt.show()

if __name__ == "__main__":
    main()
