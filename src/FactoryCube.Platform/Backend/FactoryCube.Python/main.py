#!/usr/bin/env python3
"""FactoryCube Python Pipeline CLI Entry Point"""
import argparse
import json
import logging
import sys
from pathlib import Path

from synthetic.generator import SyntheticGenerator
from validation.validator import SyntheticValidator
from train.trainer import MlTrainer
from infer.inference import InferenceEngine

logging.basicConfig(
    level=logging.INFO,
    format="%(asctime)s [%(levelname)s] %(message)s",
    handlers=[logging.StreamHandler(sys.stdout)]
)
logger = logging.getLogger("factorycube")


def main():
    parser = argparse.ArgumentParser(description="FactoryCube Python Pipeline")
    parser.add_argument("--job-type", required=True, choices=["synthetic", "validate", "train", "infer"])
    parser.add_argument("--config", required=True, help="Path to config JSON")
    parser.add_argument("--input", default="", help="Input data path")
    parser.add_argument("--output", required=True, help="Output directory")
    args = parser.parse_args()

    with open(args.config, "r", encoding="utf-8") as f:
        config = json.load(f)

    out_dir = Path(args.output)
    out_dir.mkdir(parents=True, exist_ok=True)

    if args.job_type == "synthetic":
        gen = SyntheticGenerator(config)
        df = gen.generate()
        output_path = out_dir / "synthetic.csv"
        df.to_csv(output_path, index=False)
        logger.info("Synthetic data written to %s", output_path)

    elif args.job_type == "validate":
        val = SyntheticValidator(config)
        report = val.validate(args.input, out_dir / "synthetic.csv")
        report_path = out_dir / "validation_report.json"
        with open(report_path, "w", encoding="utf-8") as f:
            json.dump(report, f, ensure_ascii=False, indent=2)
        logger.info("Validation report written to %s", report_path)

    elif args.job_type == "train":
        trainer = MlTrainer(config)
        result = trainer.train(args.input, out_dir)
        logger.info("Training completed. Artifacts in %s", out_dir)

    elif args.job_type == "infer":
        engine = InferenceEngine(config)
        result = engine.run(args.input, out_dir)
        logger.info("Inference completed. Results in %s", out_dir)


if __name__ == "__main__":
    main()
