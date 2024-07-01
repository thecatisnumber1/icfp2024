output="3072297283032850841637141062560831040929414344096048005812316929650288770628"
while [ -n "$output" ]; do
    output=$(python 9.py "$output")
    echo $output
done

