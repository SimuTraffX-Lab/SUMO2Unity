import matplotlib.pyplot as plt
import math

def main():
    input_file = "rtf_report.txt"
    times = []
    rtfs = []

    # Open and read the file
    with open(input_file, "r") as f:
        lines = f.readlines()
    
    # Skip the header line(s). Adjust if your file has a different number of header lines.
    # Assuming the first line is a header line and the second might also be a header:
    # If only one header line is present, use lines[1:].
    # If two header lines, use lines[2:].
    for line in lines[2:]:
        line = line.strip()
        if not line:
            continue
        # Split by semicolon
        parts = line.split(';')
        if len(parts) == 2:
            t = float(parts[0])
            rtf = float(parts[1])
            times.append(t)
            rtfs.append(rtf)

    # Group RTF values by their integer second
    rtf_by_second = {}
    for t, r in zip(times, rtfs):
        second = int(math.floor(t))  # Get the integer second
        if second not in rtf_by_second:
            rtf_by_second[second] = []
        rtf_by_second[second].append(r)

    # Compute the average RTF for each second
    avg_times = sorted(rtf_by_second.keys())
    avg_rtfs = [sum(rtf_by_second[s]) / len(rtf_by_second[s]) for s in avg_times]

    # Calculate the overall average RTF across all seconds
    if avg_rtfs:
        overall_avg_rtf = sum(avg_rtfs) / len(avg_rtfs)
    else:
        overall_avg_rtf = 0.0

    # Plot the averaged data
    plt.figure(figsize=(10, 2.5))
    plt.plot(avg_times, avg_rtfs, marker='o', linestyle='-', color='#0072BD', label='RTF per Second', markersize=3)

    # Set y-axis range (adjust as needed)
    plt.ylim(0.8, 1.2)

    # Draw a red line showing the overall average RTF
    plt.axhline(y=overall_avg_rtf, color='#D95319', linestyle='--', linewidth=1, label=f' Avg RTF: {overall_avg_rtf:.4f}')

    plt.title("Average Real-Time Factor by Second")
    plt.xlabel("Time (s)")
    plt.ylabel("RTF")
    plt.grid(True)
    plt.legend(fontsize=14)


    plt.xticks(range(0, 151, 50))
 
    plt.xlim(left=0)
    plt.xticks(fontsize=12)
    plt.yticks(fontsize=12)

    # Show the plot
    plt.show()

if __name__ == "__main__":
    main()
