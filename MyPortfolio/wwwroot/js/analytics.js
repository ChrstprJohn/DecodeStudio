if (typeof Chart !== "undefined") {
    const ink = "#262626";
    const muted = "#d4d4d4";
    const blue = "#1d5fbf";
    const axis = { beginAtZero: true, suggestedMax: 1, ticks: { precision: 0, maxTicksLimit: 5 } };
    const options = { maintainAspectRatio: false, animation: false };
    Chart.defaults.font.family = "Inter, system-ui, sans-serif";

    const registrations = document.getElementById("registration-chart");
    new Chart(registrations, {
        type: "line",
        data: {
            labels: JSON.parse(registrations.dataset.labels),
            datasets: [{
                label: "Registrations",
                data: JSON.parse(registrations.dataset.values),
                borderColor: blue,
                backgroundColor: blue,
                borderWidth: 2,
                pointRadius: 2
            }]
        },
        options: {
            ...options,
            plugins: { legend: { display: false } },
            scales: { x: { grid: { display: false }, ticks: { maxTicksLimit: 7 } }, y: axis }
        }
    });

    const accounts = document.getElementById("account-chart");
    const active = Number(accounts.dataset.active);
    const inactive = Number(accounts.dataset.inactive);
    new Chart(accounts, {
        type: "doughnut",
        data: {
            labels: active + inactive ? ["Active", "Inactive"] : ["No accounts"],
            datasets: [{
                data: active + inactive ? [active, inactive] : [1],
                backgroundColor: active + inactive ? [ink, muted] : ["#e5e5e5"],
                borderWidth: 0
            }]
        },
        options: {
            ...options, cutout: "77%",
            plugins: { legend: { display: false }, tooltip: { enabled: active + inactive > 0 } }
        }
    });

    const content = document.getElementById("content-chart");
    new Chart(content, {
        type: "bar",
        data: {
            labels: JSON.parse(content.dataset.labels),
            datasets: [
                { label: "Published", data: JSON.parse(content.dataset.published), backgroundColor: ink, maxBarThickness: 22 },
                { label: "Draft", data: JSON.parse(content.dataset.draft), backgroundColor: muted, maxBarThickness: 22 }
            ]
        },
        options: {
            ...options,
            indexAxis: "y",
            plugins: { legend: { align: "end", labels: { boxWidth: 10, boxHeight: 10 } } },
            scales: { x: { ...axis, stacked: true }, y: { stacked: true, grid: { display: false } } }
        }
    });
}
